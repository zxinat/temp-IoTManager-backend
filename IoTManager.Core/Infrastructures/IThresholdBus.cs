using System;
using System.Collections.Generic;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IThresholdBus
    {
        Dictionary<String, Tuple<String, int>> GetByDeviceId(String deviceId);
        String InsertThreshold(ThresholdSerializer thresholdSerializer);
        List<ThresholdSerializerDisplay> GetAllRules();
    }
}