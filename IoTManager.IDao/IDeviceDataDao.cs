using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IDeviceDataDao
    {
        List<DeviceDataModel> Get();
        DeviceDataModel GetById(String Id);
        List<DeviceDataModel> GetByDeviceId(String DeviceId);
        List<DeviceDataModel> GetNotInspected();
        Object GetLineChartData(String deviceId, String indexId);
        int GetDeviceDataAmount();
        object GetDeviceStatusById(int id);
    }
}