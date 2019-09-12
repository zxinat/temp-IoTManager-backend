using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IStateTypeDao
    {
        List<String> GetDeviceType();
        List<String> GetDeviceState();
        List<String> GetGatewayType();
        List<String> GetGatewayState();
        List<DeviceTypeModel> GetDetailedDeviceType();
    }
}