using System;
using System.Collections.Generic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using Microsoft.Extensions.Logging;

namespace IoTManager.Core
{
    public class DeviceDailyOnlineTimeBus: IDeviceDailyOnlineTimeBus
    {
        private readonly IDeviceDailyOnlineTimeDao _deviceDailyOnlineTimeDao;
        private readonly ILogger _logger;

        public DeviceDailyOnlineTimeBus(IDeviceDailyOnlineTimeDao deviceDailyOnlineTimeDao,
            ILogger<DeviceDailyOnlineTimeBus> logger)
        {
            this._deviceDailyOnlineTimeDao = deviceDailyOnlineTimeDao;
            this._logger = logger;
        }

        public List<DeviceDailyOnlineTimeModel> GetAll()
        {
            return this._deviceDailyOnlineTimeDao.GetAll();
        }

        public Double GetAverageOnlineTimeByDevice(String deviceId)
        {
            List<DeviceDailyOnlineTimeModel>
                onlineTime = this._deviceDailyOnlineTimeDao.GetOnlineTimeByDevice(deviceId);
            Double total = 0.0;
            foreach (var dm in onlineTime)
            {
                total += dm.OnlineTime;
            }

            total /= onlineTime.Count;
            return total;
        }

        public List<DeviceDailyOnlineTimeModel> SummaryAllDeviceOnlineTime(DateTime startTime, DateTime endTime)
        {
            return this._deviceDailyOnlineTimeDao.SummaryAllDeviceOnlineTime(startTime, endTime);
        }

    }
}