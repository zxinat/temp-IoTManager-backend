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
            t.ThresholdValue = double.Parse(thresholdSerializer.value);
            t.RuleName = thresholdSerializer.name;
            t.Description = thresholdSerializer.description;
            t.Severity = thresholdSerializer.severity;
            return this._thresholdDao.Create(t);
        }

        public String UpdateThreshold(int id, ThresholdSerializer thresholdSerializer)
        {
            ThresholdModel t = new ThresholdModel();
            t.IndexId = thresholdSerializer.field;
            t.DeviceId = thresholdSerializer.deviceGroup;
            t.Operator = thresholdSerializer.Operator;
            t.ThresholdValue = double.Parse(thresholdSerializer.value);
            t.RuleName = thresholdSerializer.name;
            t.Description = thresholdSerializer.description;
            t.Severity = thresholdSerializer.severity;
            t.UpdateTime = DateTime.Now;
            return this._thresholdDao.Update(id, t);
        }

        public List<ThresholdSerializer> GetAllRules(String searchType, String deviceName = "all", int page = 1, String sortColumn = "Id", String order = "asc")
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            List<ThresholdModel> thresholds = this._thresholdDao.Get(searchType, deviceName, offset, limit, sortColumn, order);
            List<ThresholdSerializer> result = new List<ThresholdSerializer>();
            foreach (ThresholdModel t in thresholds)
            {
                result.Add(new ThresholdSerializer(t));
            }

            return result;
        }

        public long GetThresholdNumber(String searchType, String deviceName = "all")
        {
            return this._thresholdDao.GetThresholdNumber(searchType, deviceName);
        }

        public String DeleteThreshold(int id)
        {
            return this._thresholdDao.Delete(id);
        }

        public int BatchDeleteThreshold(int[] ids)
        {
            return this._thresholdDao.BatchDelete(ids);
        }

        public int GetDeviceAffiliateThreshold(String deviceId)
        {
            return this._thresholdDao.GetDeviceAffiliateThreshold(deviceId);
        }
    }
}