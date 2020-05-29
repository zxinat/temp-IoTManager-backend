using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IDeviceDailyOnlineTimeDao
    {
        List<DeviceDailyOnlineTimeModel> GetAll();
        String InsertData(DeviceModel device, Double onlineTime);
        string InsertData(DeviceModel device, double onlineTime, DateTime date);
        List<DeviceDailyOnlineTimeModel> GetOnlineTimeByDevice(String deviceId);
        List<DeviceDailyOnlineTimeModel> GetDeviceOnlineTimeByTime(DateTime startTime, DateTime endTime);
        DeviceDailyOnlineTimeModel GetDeviceDailyOnlineTime(string deviceName, DateTime Date);
        double GetTotalMinutesOnline(string deviceId, DateTime startTime, DateTime endTime);
        double GetTotalMinutesOnline(string deviceId);
    }
}