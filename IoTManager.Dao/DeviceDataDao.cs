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

namespace IoTManager.Dao
{
    public sealed class DeviceDataDao: IDeviceDataDao
    {
        private readonly IMongoCollection<DeviceDataModel> _deviceData;
        private readonly IMongoCollection<AlarmInfoModel> _alarmInfo;

        public DeviceDataDao()
        {
            var client = new MongoClient(Constant.getMongoDBConnectionString());
            var database = client.GetDatabase("iotmanager");

            _deviceData = database.GetCollection<DeviceDataModel>("devicedata");
            _alarmInfo = database.GetCollection<AlarmInfoModel>("alarminfo");
        }

        public List<DeviceDataModel> Get()
        {
            //return _deviceData.Find<DeviceDataModel>(d => true).ToList();
            var query = this._deviceData.AsQueryable()
                .Where(dd => true)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(15)
                .ToList();
            return query;
        }

        public DeviceDataModel GetById(String Id)
        {
            return _deviceData.Find<DeviceDataModel>(dd => dd.Id == Id).FirstOrDefault();
        }

        public List<DeviceDataModel> GetByDeviceId(String DeviceId)
        {
            return _deviceData.Find<DeviceDataModel>(dd => dd.DeviceId == DeviceId).ToList();
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
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
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

            foreach (var a in alarmQuery)
            {
                a.DeviceId = a.Timestamp.ToString(Constant.getDateFormatString());
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

            return new
            {
                deviceName = device.DeviceName,
                hardwareDeviceID = device.HardwareDeviceId,
                deviceType = device.DeviceType,
                deviceState = device.DeviceState,
                imageUrl = device.ImageUrl,
                alarmInfo = alarmQuery,
                startTime = startTime.ToString(Constant.getDateFormatString()),
                runningTime = lastingTime.ToString("%d") + "天" + 
                              lastingTime.ToString("%h") + "小时" + 
                              lastingTime.ToString("%m") + "分钟" + 
                              lastingTime.ToString("%s") + "秒",
                alarmTimes = alarmTimes.ToString(),
                recentAlarmTime = recentAlarm.ToString(Constant.getDateFormatString())
            };
        }
    }
}