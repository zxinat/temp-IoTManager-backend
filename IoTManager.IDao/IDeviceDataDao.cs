using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IDeviceDataDao
    {
        List<DeviceDataModel> Get(String searchType, String deviceId = "all", int offset = 0, int limit = 12, String sortColumn = "Id", String order = "asc");
        DeviceDataModel GetById(String Id);
        List<DeviceDataModel> GetByDeviceId(String DeviceId);
        List<DeviceDataModel> GetNotInspected();
        Object GetLineChartData(String deviceId, String indexId);
        int GetDeviceDataAmount();
        object GetDeviceStatusById(int id, DateTime sTime, DateTime eTime);
        object GetDeviceStatistic(StatisticDurationModel statisticDurationModel);
        long GetDeviceDataNumber(String searchType, String deviceId = "all");
        String Delete(String id);
        int BatchDelete(List<String> ids);
        String Update(String id, DeviceDataModel deviceDataModel);
        object GetDayAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime);
        object GetHourAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime);
        object GetMonthAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime);
        int GetDeviceAffiliateData(String deviceId);
    }
}