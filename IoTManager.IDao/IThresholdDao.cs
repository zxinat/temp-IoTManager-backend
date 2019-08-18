using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IThresholdDao
    {
        List<ThresholdModel> GetByDeviceId(String deviceId);
        String Create(ThresholdModel thresholdModel);
        List<ThresholdModel> Get(String searchType, List<DeviceModel>devices, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc");
    }
}