using System;
using System.Collections.Generic;
using IoTManager.Model;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IThresholdBus
    {
        List<ThresholdModel> GetByDeviceId(String deviceId);
        String InsertThreshold(ThresholdSerializer thresholdSerializer);
        List<ThresholdSerializerDisplay> GetAllRules();
    }
}