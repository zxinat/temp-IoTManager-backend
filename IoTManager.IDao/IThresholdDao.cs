using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IThresholdDao
    {
        Dictionary<String, Tuple<String, double>> GetByDeviceId(String deviceId);
        String Create(ThresholdModel thresholdModel);
        List<ThresholdModel> Get();
    }
}