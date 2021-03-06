using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IDeviceDailyOnlineTimeDao
    {
        List<DeviceDailyOnlineTimeModel> GetAll();
        String InsertData(DeviceModel device, Double onlineTime);
        List<DeviceDailyOnlineTimeModel> GetOnlineTimeByDevice(String deviceId);
        List<DeviceDailyOnlineTimeModel> GetDeviceOnlineTimeByTime(DateTime startTime, DateTime endTime);
    }
}