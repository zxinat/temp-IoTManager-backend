using System;
using System.Collections.Generic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;

namespace IoTManager.Core
{
    public sealed class DeviceDataBus: IDeviceDataBus
    {
        private readonly IDeviceDataDao _deviceDataDao;
        private readonly IDeviceDao _deviceDao;
        private readonly ILogger _logger;

        public DeviceDataBus(IDeviceDataDao deviceDataDao, ILogger<DeviceDataBus> logger, IDeviceDao deviceDao)
        {
            this._deviceDataDao = deviceDataDao;
            this._logger = logger;
            this._deviceDao = deviceDao;
        }

        public List<DeviceDataSerializer> GetAllDeviceData(String searchType, String deviceId = "all", int page = 1, String sortColumn = "Id", String order = "asc")
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            List<DeviceDataModel> deviceData = this._deviceDataDao.Get(searchType, deviceId, offset, limit, sortColumn, order);
            List<DeviceDataSerializer> result = new List<DeviceDataSerializer>();
            foreach (DeviceDataModel dd in deviceData)
            {
                result.Add(new DeviceDataSerializer(dd));
            }
            return result;
        }

        public DeviceDataSerializer GetDeviceDataById(String Id)
        {
            DeviceDataModel deviceData = this._deviceDataDao.GetById(Id);
            DeviceDataSerializer result = new DeviceDataSerializer(deviceData);
            return result;
        }

        public List<DeviceDataSerializer> GetDeviceDataByDeviceId(String DeviceId)
        {
            List<DeviceDataModel> deviceData = this._deviceDataDao.GetByDeviceId(DeviceId);
            List<DeviceDataSerializer> result = new List<DeviceDataSerializer>();
            foreach (DeviceDataModel dd in deviceData)
            {
                result.Add(new DeviceDataSerializer(dd));
            }
            return result;
        }

        public Object GetLineChartData(String deviceId, String indexId)
        {
            return this._deviceDataDao.GetLineChartData(deviceId, indexId);
        }

        public int GetDeviceDataAmount()
        {
            return this._deviceDataDao.GetDeviceDataAmount();
        }

        public object GetDeviceStatusById(int id)
        {
            return this._deviceDataDao.GetDeviceStatusById(id);
        }

        public long GetDeviceDataNumber(String searchType, String deviceId = "all")
        {
            return this._deviceDataDao.GetDeviceDataNumber(searchType, deviceId);
        }

        public String DeleteDeviceData(String id)
        {
            return this._deviceDataDao.Delete(id);
        }

        public int BatchDeleteDeviceData(List<String> ids)
        {
            return this._deviceDataDao.BatchDelete(ids);
        }

        public String UpdateDeviceData(String id, DeviceDataSerializer deviceDataSerializer)
        {
            DeviceDataModel deviceDataModel = new DeviceDataModel();
            deviceDataModel.Id = deviceDataSerializer.id;
            deviceDataModel.DeviceId = deviceDataSerializer.deviceId;
            deviceDataModel.IndexName = deviceDataSerializer.indexName;
            deviceDataModel.IndexId = deviceDataSerializer.indexId;
            deviceDataModel.IndexUnit = deviceDataSerializer.indexUnit;
            deviceDataModel.IndexType = deviceDataSerializer.indexType;
            deviceDataModel.IndexValue = deviceDataSerializer.indexValue;
            return this._deviceDataDao.Update(id, deviceDataModel);
        }

        public object GetDayAggregateData(String deviceId, String indexId)
        {
            return this._deviceDataDao.GetDayAggregateData(deviceId, indexId);
        }

        public object GetMultipleLineChartData(String deviceId, List<String> fields)
        {
            List<object> data = new List<object>();
            foreach (String f in fields)
            {
                data.Add(this._deviceDataDao.GetLineChartData(deviceId, f));
            }

            return data;
        }
    }
}