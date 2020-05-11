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
        /* 定时器任务：统计每个设备每天的在线时长
         * 问题：如果设备一天只上报一条数据，则判断在线时长为0
         */
        [Invoke(Begin = "2019-6-16 23:58", Interval = 1000*3600*24,SkipWhileExecuting = true)]
        public void Run()
        {
            _logger.LogInformation("统计所有设备每天在线时长...");

            List<DeviceModel> devices = _deviceDao.Get("all");
            int deviceNumber = devices.Count();
            foreach (var d in devices)
            {
                //_logger.LogInformation("剩余 " + (deviceNumber - 1).ToString() + " 设备");
                _logger.LogInformation("设备名：" + d.DeviceName);
                //获取设备最早的一条数据
                DeviceDataModel deviceData = _deviceDataDao.ListByDeviceNameASC(d.DeviceName, 1).FirstOrDefault();
                string text = deviceData != null ? d.DeviceName + "设备数据时间：" + deviceData.Timestamp.ToString()
                    : d.DeviceName + "设备没有数据";
                _logger.LogInformation(text);
                //计算天数
                //起止时间改成00:00-23:59
                DateTime nowadays = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                double totalDays = deviceData != null ? (nowadays - new DateTime(deviceData.Timestamp.Year,deviceData.Timestamp.Month, deviceData.Timestamp.Day)).TotalDays
                    : 0;
                string text1 = deviceData != null ? ("最早的设备数据时间: " + deviceData.Timestamp + " 距离现在有 " + totalDays + " 天")
                    : "没有设备数据";
                _logger.LogInformation(text1);
                for (double i = 0; i < totalDays; i++)
                {
                    DateTime sTime = new DateTime(deviceData.Timestamp.Year, deviceData.Timestamp.Month, deviceData.Timestamp.Day);
                    _logger.LogInformation(sTime.AddDays(i).ToString());
                    //List<DeviceDataModel> deviceDatas = _deviceDataDao.ListByNameTimeASC(d.DeviceName, sTime.AddDays(i), sTime.AddDays(i+1), 20000);
                    DeviceDataModel firstdeviceData = _deviceDataDao.ListByNameTimeASC(d.DeviceName, sTime.AddDays(i), sTime.AddDays(i + 1), 1).FirstOrDefault();
                    DeviceDataModel lastdeviceData = _deviceDataDao.ListByNameTimeDSC(d.DeviceName, sTime.AddDays(i), sTime.AddDays(i + 1), 1).FirstOrDefault();
                    //_logger.LogInformation(deviceDatas.Count().ToString());
                    if (firstdeviceData != null)
                    {
                        double onlineTime = (lastdeviceData.Timestamp - firstdeviceData.Timestamp).TotalMinutes;
                        _logger.LogInformation("在线时长 " + onlineTime + " 分钟");
                        _logger.LogInformation("插入数据到DailyOnlineTime表...");
                        _deviceDailyOnlineTimeDao.InsertData(d, onlineTime, sTime.AddDays(i));
                    }
                }
            }
            /*
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
            */
        }
    }
}