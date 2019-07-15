using System;
using System.Collections.Generic;
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

        public DeviceDataDao()
        {
            var client = new MongoClient(Constant.getMongoDBConnectionString());
            var database = client.GetDatabase("iotmanager");

            _deviceData = database.GetCollection<DeviceDataModel>("devicedata");
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
            List<int> chartValue = new List<int>();
            List<String> xAxises = new List<string>();

            var query = this._deviceData.AsQueryable()
                .Where(dd => (dd.DeviceId == deviceId && dd.IndexId == indexId))
                .OrderByDescending(dd => dd.Timestamp)
                .Take(7)
                .ToList();

            foreach (var dd in query)
            {
                chartValue.Add(int.Parse(dd.IndexValue));
                xAxises.Add(dd.Timestamp.ToString(Constant.getLineChartDateFormatString()));
            }

            xAxises.Reverse();
            chartValue.Reverse();

            return new {xAxis = xAxises, series = chartValue};
        }
    }
}