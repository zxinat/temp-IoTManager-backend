using System;
using System.Collections.Generic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;

namespace IoTManager.Core
{
    public sealed class ThresholdBus: IThresholdBus
    {
        private readonly IThresholdDao _thresholdDao;
        private readonly IDeviceDao _deviceDao;
        private readonly ILogger _logger;

        public ThresholdBus(IThresholdDao thresholdDao, ILogger<ThresholdBus> logger, IDeviceDao deviceDao)
        {
            this._thresholdDao = thresholdDao;
            this._logger = logger;
            this._deviceDao = deviceDao;
        }
        
        public List<ThresholdModel> GetByDeviceId(String deviceId)
        {
            return _thresholdDao.GetByDeviceId(deviceId);
        }

        public String InsertThreshold(ThresholdSerializer thresholdSerializer)
        {
            ThresholdModel t = new ThresholdModel();
            t.IndexId = thresholdSerializer.field;
            t.DeviceId = thresholdSerializer.deviceGroup;
            t.Operator = thresholdSerializer.Operator;
            t.ThresholdValue = int.Parse(thresholdSerializer.value);
            t.RuleName = thresholdSerializer.name;
            t.Description = thresholdSerializer.description;
            t.Severity = thresholdSerializer.severity;
            return this._thresholdDao.Create(t);
        }

        public List<ThresholdSerializerDisplay> GetAllRules(String searchType, String city, String factory, String workshop, int page = 1, String sortColumn = "Id", String order = "asc")
        {
            List<DeviceModel> devices = new List<DeviceModel>();
            if (searchType == "search")
            {
                devices = this._deviceDao.GetByWorkshop(city, factory, workshop);
            }
            int offset = (page - 1) * 12;
            int limit = 12;
            List<ThresholdModel> thresholds = this._thresholdDao.Get(searchType, devices, offset, limit, sortColumn, order);
            List<ThresholdSerializerDisplay> result = new List<ThresholdSerializerDisplay>();
            foreach (ThresholdModel t in thresholds)
            {
                result.Add(new ThresholdSerializerDisplay(t));
            }

            return result;
        }

        public long GetThresholdNumber(String searchType, String city, String factory, String workshop)
        {
            List<DeviceModel> devices = new List<DeviceModel>();
            if (searchType == "search")
            {
                devices = this._deviceDao.GetByWorkshop(city, factory, workshop);
            }

            return this._thresholdDao.GetThresholdNumber(searchType, devices);
        }
    }
}