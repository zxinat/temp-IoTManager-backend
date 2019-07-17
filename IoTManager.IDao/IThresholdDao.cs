using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IThresholdDao
    {
        List<ThresholdModel> GetByDeviceId(String deviceId);
        String Create(ThresholdModel thresholdModel);
        List<ThresholdModel> Get();
    }
}