using System;
using System.Collections.Generic;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IStateTypeBus
    {
        List<object> GetAllDeviceTypes();
        List<object> GetAllDeviceStates();
        List<object> GetAllGatewayTypes();
        List<object> GetAllGatewayStates();
        List<DeviceTypeSerializer> GetDetailedDeviceTypes(int pageMode = 0, int page = 1, String sortColumn = "id", String order = "asc");
        String AddDeviceType(DeviceTypeSerializer deviceTypeSerializer);
        String UpdateDeviceType(int id, DeviceTypeSerializer deviceTypeSerializer);
        String DeleteDeviceType(int id);
        int GetDeviceTypeAffiliateDevice(int id);
        long GetDetailedDeviceTypeNumber();
    }
}