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
        private readonly ILogger _logger;

        public ThresholdBus(IThresholdDao thresholdDao, ILogger<ThresholdBus> logger)
        {
            this._thresholdDao = thresholdDao;
            this._logger = logger;
        }
        
        public Dictionary<String, Tuple<String, double>> GetByDeviceId(String deviceId)
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
            return this._thresholdDao.Create(t);
        }

        public List<ThresholdSerializerDisplay> GetAllRules()
        {
            List<ThresholdModel> thresholds = this._thresholdDao.Get();
            List<ThresholdSerializerDisplay> result = new List<ThresholdSerializerDisplay>();
            foreach (ThresholdModel t in thresholds)
            {
                result.Add(new ThresholdSerializerDisplay(t));
            }

            return result;
        }
    }
}