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
using System.Linq.Dynamic;

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

        public List<DeviceDataModel> Get(String searchType, String deviceId = "all", int offset = 0, int limit = 12, String sortColumn = "Id", String order = "asc")
        {
            if (searchType == "all")
            {
                return this._deviceData.AsQueryable()
                    .Where(dd => true)
                    .ToList();
            }
            else
            {
                var query = deviceId != "all"
                    ? this._deviceData.AsQueryable()
                        .Where(dd => dd.DeviceId == deviceId)
                    : this._deviceData.AsQueryable()
                        .Where(dd => true);
                var beforeOrder = query.OrderByDescending(dd => dd.Timestamp)
                    .Take(60)
                    .ToList();
                var ordered = order == "asc"
                    ? beforeOrder.OrderBy(dd => dd.GetType().GetProperty(sortColumn).GetValue(dd))
                    : beforeOrder.OrderByDescending(dd => dd.GetType().GetProperty(sortColumn).GetValue(dd));
                return ordered.Skip(offset)
                    .Take(limit)
                    .ToList();
            }
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
                chartValue.Add(dd.IndexValue);
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
                startTime = startTime == DateTime.MinValue ? "未收到数据" : startTime.ToString(Constant.getDateFormatString()),
                runningTime = lastingTime == TimeSpan.Zero ? "未收到数据" : lastingTime.ToString("%d") + "天" + 
                              lastingTime.ToString("%h") + "小时" + 
                              lastingTime.ToString("%m") + "分钟" + 
                              lastingTime.ToString("%s") + "秒",
                alarmTimes = alarmTimes.ToString(),
                recentAlarmTime = recentAlarm == DateTime.MinValue ? "未收到数据" : recentAlarm.ToString(Constant.getDateFormatString()),
                rules = ruleResult
            };
        }

        public object GetDeviceStatistic(StatisticDurationModel statisticDurationModel)
        {
            return null;
        }

        public long GetDeviceDataNumber(String searchType, String deviceId = "all")
        {
            if (searchType == "search")
            {
                if (deviceId != "all")
                {
                    var query = this._deviceData.AsQueryable()
                        .Where(dd => dd.DeviceId == deviceId)
                        .Take(60)
                        .ToList();
                    return query.Count;
                }
                else
                {
                    var query = this._deviceData.AsQueryable()
                        .Where(dd => true)
                        .Take(60)
                        .ToList();
                    return query.Count;
                }
            }
            else
            {
                var query = this._deviceData.AsQueryable()
                    .Where(dd => true)
                    .ToList();
                return query.Count;
            }
        }

        public String Delete(String id)
        {
            var filter = Builders<DeviceDataModel>.Filter.Eq("_id", new ObjectId(id));
            var result = this._deviceData.DeleteOne(filter);
            return result.DeletedCount == 1 ? "success" : "error";
        }

        public int BatchDelete(List<String> ids)
        {
            int num = 0;
            foreach (String id in ids)
            {
                var filter = Builders<DeviceDataModel>.Filter.Eq("_id", new ObjectId(id));
                var result = this._deviceData.DeleteOne(filter);
                num = num + 1;
            }
            return num;
        }

        public String Update(String id, DeviceDataModel deviceDataModel)
        {
            var filter = Builders<DeviceDataModel>.Filter.Eq("_id", new ObjectId(id));
            var update = Builders<DeviceDataModel>.Update.Set("IndexValue", deviceDataModel.IndexValue.ToString());
            var result = _deviceData.UpdateOne(filter, update);
            return result.ModifiedCount == 1 ? "success" : "error";
        }
    }
}