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
        List<ThresholdSerializerDisplay> GetAllRules(String searchType, String deviceName = "all", int page = 1, String sortColumn = "id", String order = "asc");
        long GetThresholdNumber(String searchType, String deviceName = "all");
    }
}