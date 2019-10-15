using System;
using System.Collections.Generic;
using IoTManager.Model;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IDeviceDataBus
    {
        List<DeviceDataSerializer> GetAllDeviceData(String searchType, String deviceId = "all", int page = 1, String sortColumn = "Id", String order = "asc");
        DeviceDataSerializer GetDeviceDataById(String Id);
        List<DeviceDataSerializer> GetDeviceDataByDeviceId(String DeviceId);
        Object GetLineChartData(String deviceId, String indexId);
        int GetDeviceDataAmount();
        object GetDeviceStatusById(int id, DateTime sTime, DateTime eTime);
        long GetDeviceDataNumber(String searchType, String deviceId = "all");
        String DeleteDeviceData(String id);
        int BatchDeleteDeviceData(List<String> ids);
        String UpdateDeviceData(String id, DeviceDataSerializer deviceDataSerializer);
        object GetDayAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime, String scale);
        object GetMultipleLineChartData(String deviceId, List<String> fields);
        object GetDashboardDeviceStatus();
        int GetDeviceAffiliateData(String deviceId);
        object GetReportByRegion(String factoryName, DateTime startTime, DateTime endTime);
        object GetReportByTime(DateTime startTime, DateTime endTime);
        object GetReportByType(DateTime startTime, DateTime endTime);
        object GetReportByTag(DateTime startTime, DateTime endTime);
    }
}