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
        List<DeviceTypeSerializer> GetDetailedDeviceTypes();
        String AddDeviceType(DeviceTypeSerializer deviceTypeSerializer);
        String UpdateDeviceType(int id, DeviceTypeSerializer deviceTypeSerializer);
        String DeleteDeviceType(int id);
    }
}