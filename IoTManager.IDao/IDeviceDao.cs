using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IDeviceDao
    {
        List<DeviceModel> Get(String searchType, int offset, int limit, String sortColumn, String order, String city, String factory, String workshop);
        DeviceModel GetById(int id);
        List<DeviceModel> GetByDeviceName(String deviceName);
        List<DeviceModel> GetByDeviceId(String deviceId);
        String Create(DeviceModel deviceModel);
        String Update(int id, DeviceModel deviceModel);
        String Delete(int id);
        int BatchDelete(int[] ids);
        List<DeviceModel> GetByWorkshop(String city, String factory, String workshop);
        int GetDeviceAmount();
        List<object> GetDeviceTree(String city, String factory);
        String CreateDeviceType(String deviceType);
        long GetDeviceNumber();
    }
}
