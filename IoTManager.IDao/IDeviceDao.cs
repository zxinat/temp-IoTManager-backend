using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IDeviceDao
    {
        List<DeviceModel> Get(String searchType, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc", String city = "all", String factory = "all", String workshop = "all");
        DeviceModel GetById(int id);
        List<DeviceModel> GetByDeviceName(String deviceName);
        List<DeviceModel> GetByFuzzyDeviceId(String deviceId);
        DeviceModel GetByDeviceId(String deviceId);
        String Create(DeviceModel deviceModel);
        String Update(int id, DeviceModel deviceModel);
        String Delete(int id);
        int BatchDelete(int[] ids);
        List<DeviceModel> GetByWorkshop(String city, String factory, String workshop);
        int GetDeviceAmount();
        List<object> GetDeviceTree(String city, String factory);
        String CreateDeviceType(String deviceType);
        long GetDeviceNumber(String searchType, String city="all", String factory="all", String workshop="all");
        List<DeviceModel> GetDeviceByTag(String tag);
        List<String> GetAllTag();
        Object SetDeviceTag(int deviceId, List<String> tagId);
        List<DeviceModel> GetByDeviceType(String deviceType);
        String SetDeviceOnlineStatus(String id, String status);
        List<String> GetDeviceTag(int id);
        String AddTag(String tagName);
        String DeleteTag(String tagName);
        int FindTagAffiliate(String tagName);
        int FindDeviceIdExist(String deviceId);
    }
}
