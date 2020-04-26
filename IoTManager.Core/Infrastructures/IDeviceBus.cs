using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Http;
namespace IoTManager.Core.Infrastructures
{
    public interface IDeviceBus
    {
        List<DeviceSerializer> GetAllDevices(String searchType, int page, String sortColumn, String order, String city, String factory, String workshop);
        DeviceSerializer GetDeviceById(int id);
        List<DeviceSerializer> GetDevicesByDeviceName(String deviceName);
        List<DeviceSerializer> GetDevicesByFuzzyDeviceId(String deviceId);
        DeviceSerializer GetDeviceByDeviceId(String deviceId);
        String CreateNewDevice(DeviceSerializer deviceSerializer);
        String UpdateDevice(int id, DeviceSerializer deviceSerializer);
        String DeleteDevice(int id);
        int BatchDeleteDevice(int[] id);
        List<DeviceSerializer> GetDeviceByWorkshop(String city, String factory, String workshop);
        int GetDeviceAmount();
        List<object> GetDeviceTree(String city, String factory);
        String CreateDeviceType(String deviceType);
        long GetDeviceNumber(String searchType, String city="all", String factory="all", String workshop="all");
        List<object> GetFieldOptions();
        String UploadPicture(PictureUploadSerializer pic);
        String GetPicture(String deviceId);
        List<DeviceSerializer> GetDeviceByCity(String cityName, String factoryName, String workshopName);
        List<DeviceSerializer> GetDeviceByTag(String tag);
        List<String> GetAllTag();
        Object SetDeviceTag(int deviceId, List<String> tagId);
        List<String> GetDeviceTag(int id);
        String AddTag(String tagName);
        String DeleteTag(String tagName);
        int FindTagAffiliate(String tagName);
        int FindDeviceIdExist(String deviceId);
        int UpdateLastConnectionTimeByDeviceId(String deviceId);
        Object GetDeviceLocationByDeviceName(String deviceName);
        int GetTotalAlarmInfo(string deviceId);
        string UpdateTotalAlarmInfo(string deviceId, int count);
    }
}
