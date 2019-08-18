using System;
using System.Collections.Generic;
using IoTManager.Model;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IDeviceDataBus
    {
        List<DeviceDataSerializer> GetAllDeviceData(String searchType, int page, String sortColumn, String order, String city, String factory, String workshop);
        DeviceDataSerializer GetDeviceDataById(String Id);
        List<DeviceDataSerializer> GetDeviceDataByDeviceId(String DeviceId);
        Object GetLineChartData(String deviceId, String indexId);
        int GetDeviceDataAmount();
        object GetDeviceStatusById(int id);
    }
}