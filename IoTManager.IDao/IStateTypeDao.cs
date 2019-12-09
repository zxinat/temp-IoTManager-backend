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
        List<DeviceTypeModel> GetDetailedDeviceType(int pageMode = 0, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc");
        String AddDeviceType(DeviceTypeModel deviceTypeModel);
        String UpdateDeviceType(int id, DeviceTypeModel deviceTypeModel);
        String DeleteDeviceType(int id);
        int GetDeviceTypeAffiliateDevice(int id);
        long GetDetailedDeviceTypeNumber();
        DeviceTypeModel GetDeviceTypeByName(String name);
    }
}