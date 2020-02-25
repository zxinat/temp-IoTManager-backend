using System;
using System.Collections.Generic;
using System.Linq;
using IoTManager.IDao;
using IoTManager.Model;
using Microsoft.Extensions.Logging;
using Pomelo.AspNetCore.TimedJob;

namespace IoTManager.Core.Jobs
{
    public class ReportJob: Job
    {
        private readonly IDeviceDataDao _deviceDataDao;
        private readonly IDeviceDao _deviceDao;
        private readonly IDeviceDailyOnlineTimeDao _deviceDailyOnlineTimeDao;
        private readonly ILogger _logger;

        public ReportJob(IDeviceDataDao deviceDataDao,
            IDeviceDao deviceDao,
            IDeviceDailyOnlineTimeDao deviceDailyOnlineTimeDao,
            ILogger<ReportJob> logger)
        {
            this._deviceDataDao = deviceDataDao;
            this._deviceDao = deviceDao;
            this._deviceDailyOnlineTimeDao = deviceDailyOnlineTimeDao;
            this._logger = logger;
        }

        [Invoke(Begin = "2019-6-16 23:58", Interval = 1000 * 86400, SkipWhileExecuting = true)]
        public void Run()
        {
            List<DeviceDataModel> todayData = this._deviceDataDao.GetByDate(DateTime.Now);
            Dictionary<String, List<DateTime>> deviceKV = new Dictionary<string, List<DateTime>>(); 
            foreach (var i in todayData)
            {
                if (!deviceKV.ContainsKey(i.DeviceId))
                {
                    deviceKV.Add(i.DeviceId, new List<DateTime>());
                    deviceKV[i.DeviceId].Add(i.Timestamp);
                }
                else
                {
                    deviceKV[i.DeviceId].Add(i.Timestamp);
                }
            }
            foreach (var i in deviceKV.Keys)
            {
                DateTime first = deviceKV[i].Min();
                DateTime last = deviceKV[i].Max();
                var onlineTime = last - first;
                this._deviceDailyOnlineTimeDao.InsertData(this._deviceDao.GetByDeviceId(i), onlineTime.TotalMinutes);
            }
        }
    }
}