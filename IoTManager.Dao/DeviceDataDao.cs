using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class DeviceDataDao: IDeviceDataDao
    {
        private readonly IMongoCollection<DeviceDataModel> _deviceData;
        private readonly IMongoCollection<AlarmInfoModel> _alarmInfo;
        private readonly IThresholdDao _thresholdDao;

        public DeviceDataDao(IThresholdDao thresholdDao)
        {
            var client = new MongoClient(Constant.getMongoDBConnectionString());
            var database = client.GetDatabase("iotmanager");
            _deviceData = database.GetCollection<DeviceDataModel>("devicedata");
            _alarmInfo = database.GetCollection<AlarmInfoModel>("alarminfo");
            this._thresholdDao = thresholdDao;
        }

        public List<DeviceDataModel> Get(String searchType, List<DeviceModel> devices, int offset = 0, int limit = 12, String sortColumn = "Id", String order = "asc")
        {
            //return _deviceData.Find<DeviceDataModel>(d => true).ToList();
            List<DeviceDataModel> deviceList = new List<DeviceDataModel>();

            if (searchType == "search")
            {
                foreach (var device in devices)
                {
                    var query = this._deviceData.AsQueryable()
                        .Where(dd => dd.DeviceId == device.HardwareDeviceId)
                        .ToList();
                    foreach (var q in query)
                    {
                        deviceList.Add(q);
                    }
                }
            }
            else
            {
                var query = this._deviceData.AsQueryable()
                    .Where(dd => true)
                    .ToList();
                foreach (var q in query)
                {
                    deviceList.Add(q);
                }
            }
            List<DeviceDataModel> result = new List<DeviceDataModel>();
            if (order != "no" && sortColumn != "no")
            {
                if (sortColumn == "DeviceId")
                {
                    if (order == "asc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderBy(dd => dd.DeviceId)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderByDescending(dd => dd.DeviceId)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "IndexId")
                {
                    if (order == "asc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderBy(dd => dd.IndexId)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderByDescending(dd => dd.IndexId)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "IndexName")
                {
                    if (order == "asc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderBy(dd => dd.IndexName)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderByDescending(dd => dd.IndexName)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "Timestamp")
                {
                    if (order == "asc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderBy(dd => dd.Timestamp)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderByDescending(dd => dd.Timestamp)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "Id")
                {
                    if (order == "asc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderBy(dd => dd.Id)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = deviceList.AsQueryable()
                            .OrderByDescending(dd => dd.Id)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
            }
            return result;
        }

        public DeviceDataModel GetById(String Id)
        {
            return _deviceData.Find<DeviceDataModel>(dd => dd.Id == Id).FirstOrDefault();
        }

        public List<DeviceDataModel> GetByDeviceId(String DeviceId)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == DeviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(20)
                .ToList();
            return query;
        }

        public List<DeviceDataModel> GetAllDataByDeviceId(String DeviceId)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == DeviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .ToList();
            return query;
        }

        public List<DeviceDataModel> GetNotInspected()
        {
            List<DeviceDataModel> deviceDataModels = _deviceData.Find<DeviceDataModel>(dd => dd.Inspected == "No").ToList();
            var filter = Builders<DeviceDataModel>.Filter.Eq("Inspected", "No");
            var update = Builders<DeviceDataModel>.Update.Set("Inspected", "Yes");
            var result = _deviceData.UpdateMany(filter, update);
            return deviceDataModels;
        }

        public Object GetLineChartData(String deviceId, String indexId)
        {
            List<double> chartValue = new List<double>();
            List<String> xAxises = new List<string>();

            var query = this._deviceData.AsQueryable()
                .Where(dd => (dd.DeviceId == deviceId && dd.IndexId == indexId))
                .OrderByDescending(dd => dd.Timestamp)
                .Take(7)
                .ToList();

            foreach (var dd in query)
            {
                chartValue.Add(double.Parse(dd.IndexValue));
                xAxises.Add(dd.Timestamp.ToString(Constant.getLineChartDateFormatString()));
            }

            xAxises.Reverse();
            chartValue.Reverse();

            return new {xAxis = xAxises, series = chartValue};
        }

        public int GetDeviceDataAmount()
        {
            List<DeviceDataModel> deviceData = _deviceData.Find<DeviceDataModel>(dd => true).ToList();
            return deviceData.Count;
        }

        public object GetDeviceStatusById(int id)
        {
            String deviceId = "";
            DeviceModel device = new DeviceModel();
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                device = connection.Query<DeviceModel>("select * from device where id=@did", new {did = id})
                    .FirstOrDefault();
                deviceId = device.HardwareDeviceId;
            }

            var lastQuery = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(1)
                .ToList();
            
            var firstQuery = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .OrderBy(dd => dd.Timestamp)
                .Take(1)
                .ToList();

            var alarmTimeQuery = this._alarmInfo.AsQueryable()
                .Where(ai => ai.DeviceId == deviceId)
                .ToList();

            var recentAlarmQuery = this._alarmInfo.AsQueryable()
                .Where(ai => ai.DeviceId == deviceId)
                .OrderByDescending(ai => ai.Timestamp)
                .Take(1)
                .ToList();

            var alarmQuery = this._alarmInfo.AsQueryable()
                .Where(ai => ai.DeviceId == deviceId)
                .OrderByDescending(ai => ai.Timestamp)
                .Take(10)
                .ToList();

            var deviceDataQuery = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(10)
                .ToList();

            foreach (var a in alarmQuery)
            {
                a.DeviceId = a.Timestamp.ToString(Constant.getDateFormatString());
            }

            foreach (var d in deviceDataQuery)
            {
                d.DeviceId = d.Timestamp.ToString(Constant.getDateFormatString());
            }

            DeviceDataModel lastDeviceData = new DeviceDataModel();
            DeviceDataModel firstDeviceData = new DeviceDataModel();
            DateTime startTime = DateTime.MinValue;
            TimeSpan lastingTime = TimeSpan.Zero;
            ;
            if (lastQuery.Count > 0 && firstQuery.Count > 0)
            {
                lastDeviceData = lastQuery[0];
                firstDeviceData = firstQuery[0];
                startTime = firstDeviceData.Timestamp;
                lastingTime = lastDeviceData.Timestamp - firstDeviceData.Timestamp;
            }
            int alarmTimes = alarmTimeQuery.Count;
            DateTime recentAlarm = DateTime.MinValue;
            if (recentAlarmQuery.Count > 0)
            {
                recentAlarm = recentAlarmQuery[0].Timestamp;
            }

            List<ThresholdModel> thresholdModels = this._thresholdDao.GetByDeviceId(deviceId);
            List<object> ruleResult = new List<object>();
            Dictionary<String, String> opDict = new Dictionary<string, string>();
            opDict.Add("greater", ">");
            opDict.Add("equal", "=");
            opDict.Add("less", "<");
            foreach (ThresholdModel t in thresholdModels)
            {
                ruleResult.Add(new {name=t.RuleName, description=t.Description, conditionString=t.IndexId + opDict[t.Operator.ToString()] + t.ThresholdValue.ToString(), severity=t.Severity});
            }

            return new
            {
                deviceName = device.DeviceName,
                hardwareDeviceID = device.HardwareDeviceId,
                deviceType = device.DeviceType,
                deviceState = device.DeviceState,
                imageUrl = device.ImageUrl,
                alarmInfo = alarmQuery,
                deviceData = deviceDataQuery,
                startTime = startTime.ToString(Constant.getDateFormatString()),
                runningTime = lastingTime.ToString("%d") + "天" + 
                              lastingTime.ToString("%h") + "小时" + 
                              lastingTime.ToString("%m") + "分钟" + 
                              lastingTime.ToString("%s") + "秒",
                alarmTimes = alarmTimes.ToString(),
                recentAlarmTime = recentAlarm.ToString(Constant.getDateFormatString()),
                rules = ruleResult
            };
        }

        public object GetDeviceStatistic(StatisticDurationModel statisticDurationModel)
        {
            return null;
        }

        public long GetDeviceDataNumber(String searchType, List<DeviceModel> devices)
        {
            long number = 0;
            if (searchType == "search")
            {
                foreach (var device in devices)
                {
                    var query = this._deviceData.AsQueryable()
                        .Where(dd => dd.DeviceId == device.HardwareDeviceId)
                        .ToList();
                    number += query.Count;
                }
            }
            else
            {
                var query = this._deviceData.AsQueryable()
                    .Where(dd => true)
                    .ToList();
                number = query.Count;
            }

            return number;
        }
    }
}