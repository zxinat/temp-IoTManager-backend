using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.Core.Infrastructures
{
    public interface IDeviceDailyOnlineTimeBus
    {
        List<DeviceDailyOnlineTimeModel> GetAll();
        Double GetAverageOnlineTimeByDevice(String deviceId);
    }
}