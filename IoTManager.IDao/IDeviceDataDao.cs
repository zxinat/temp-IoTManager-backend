using System;
using System.Collections.Generic;
using IoTManager.Model;
using IoTManager.Model.DataReceiver;

namespace IoTManager.IDao
{
    public interface IDeviceDataDao
    {
        List<DeviceDataModel> Get(String searchType, String deviceId = "all", int offset = 0, int limit = 12, String sortColumn = "Id", String order = "asc");
        DeviceDataModel GetById(String Id);
        List<DeviceDataModel> GetByDeviceId(String deviceId);
        List<DeviceDataModel> GetByDeviceName(String deviceName, int count);
        List<DeviceDataModel> ListByDeviceNameASC(string deviceName, int count);
        List<DeviceDataModel> ListByNameTimeASC(string deviceName, DateTime startTime, DateTime endTime, int count);
        List<DeviceDataModel> ListByNameTimeDSC(string deviceName, DateTime startTime, DateTime endTime, int count);
        DeviceDataModel GetLastDataByDeviceId(string deviceId);
        List<DeviceDataModel> ListByIdTimeDSC(string deviceId, DateTime startTime, DateTime endTime, int count);
        List<DeviceDataModel> GetByDeviceId20(String DeviceId);
        List<DeviceDataModel> GetByDeviceId100(String deviceId);
        List<DeviceDataModel> GetNotInspected();
        Object GetLineChartData(String deviceId, String indexId);
        int GetDeviceDataAmount();
        object GetDeviceStatusById(int id, DateTime sTime, DateTime eTime);
        object GetDeviceStatistic(StatisticDurationModel statisticDurationModel);
        long GetDeviceDataNumber(String searchType, String deviceId = "all");
        String Delete(String id);
        int BatchDelete(List<String> ids);
        String Update(String id, DeviceDataModel deviceDataModel);
        List<StatisticDataModel> GetDayStatisticAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime);
        List<StatisticDataModel> GetDayAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime, String scale);
        List<StatisticDataModel> GetHourAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime);
        List<StatisticDataModel> GetMonthAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime);
        int GetDeviceAffiliateData(String deviceId);
        int GetFieldAffiliateData(String fieldId);
        List<DeviceDataModel> GetByDate(DateTime date);
        List<DeviceDataModel> ListNewData(string deviceId, int second, string monitorId = "all");
        object GetDashboardDeviceStatus();
        string Create(DeviceDataModel deviceData);
        List<DeviceDataModel> ListByDeviceIdAndIndexId(string deviceId, string indexId, DateTime startTime, DateTime endTime, int count=0);
        List<DeviceDataModel> ListByDeviceIdAndValue(string deviceId, int indexValue, int count);
    }
}