using System;
using System.Collections.Generic;
using IoTManager.Model;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IDeviceDataBus
    {
        List<DeviceDataSerializer> GetAllDeviceData(String searchType, String deviceId = "all", int page = 1, String sortColumn = "id", String order = "asc");
        DeviceDataSerializer GetDeviceDataById(String Id);
        List<DeviceDataSerializer> GetDeviceDataByDeviceId(String DeviceId);
        Object GetLineChartData(String deviceId, String indexId);
        int GetDeviceDataAmount();
        object GetDeviceStatusById(int id);
        long GetDeviceDataNumber(String searchType, String deviceId = "all");
        String DeleteDeviceData(String id);
        int BatchDeleteDeviceData(List<String> ids);
        String UpdateDeviceData(String id, DeviceDataSerializer deviceDataSerializer);
    }
}