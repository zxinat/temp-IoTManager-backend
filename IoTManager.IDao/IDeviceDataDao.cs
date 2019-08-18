using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IDeviceDataDao
    {
        List<DeviceDataModel> Get(String searchType, List<DeviceModel> devices, int offset = 0, int limit = 12, String sortColumn = "Id", String order = "asc");
        DeviceDataModel GetById(String Id);
        List<DeviceDataModel> GetByDeviceId(String DeviceId);
        List<DeviceDataModel> GetNotInspected();
        Object GetLineChartData(String deviceId, String indexId);
        int GetDeviceDataAmount();
        object GetDeviceStatusById(int id);
        object GetDeviceStatistic(StatisticDurationModel statisticDurationModel);
    }
}