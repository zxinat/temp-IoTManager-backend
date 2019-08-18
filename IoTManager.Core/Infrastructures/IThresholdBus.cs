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
        List<ThresholdSerializerDisplay> GetAllRules(String searchType, String city, String factory, String workshop, int page = 1, String sortColumn = "Id", String order = "asc");
        long GetThresholdNumber(String searchType, String city, String factory, String workshop);
    }
}