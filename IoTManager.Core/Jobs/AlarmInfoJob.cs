using System;
using System.Collections.Generic;
using System.Linq;
using IoTManager.IDao;
using IoTManager.Model;
using Microsoft.Extensions.Logging;
using Pomelo.AspNetCore.TimedJob;

namespace IoTManager.Core.Jobs
{
    public class AlarmInfoJob: Job
    {
        private readonly IDeviceDataDao _deviceDataDao;
        private readonly IAlarmInfoDao _alarmInfoDao;
        private readonly IThresholdDao _thresholdDao;
        private readonly ISeverityDao _severityDao;
        private readonly ILogger _logger;

        public AlarmInfoJob(IDeviceDataDao deviceDataDao, IAlarmInfoDao alarmInfoDao, IThresholdDao thresholdDao, ISeverityDao severityDao, ILogger<AlarmInfoJob> logger)
        {
            this._deviceDataDao = deviceDataDao;
            this._alarmInfoDao = alarmInfoDao;
            this._thresholdDao = thresholdDao;
            this._severityDao = severityDao;
            this._logger = logger;
        }

        [Invoke(Begin = "2019-6-16 16:20", Interval = 1000 * 10, SkipWhileExecuting = true)]
        public void Run()
        {
            List<DeviceDataModel> dataNotInspected = _deviceDataDao.GetNotInspected();
            Dictionary<String, List<DeviceDataModel>> sortedData = new Dictionary<string, List<DeviceDataModel>>();
            List<String> deviceIds = new List<string>();
            foreach (DeviceDataModel d in dataNotInspected)
            {
                if (!sortedData.ContainsKey(d.DeviceId))
                {
                    sortedData.Add(d.DeviceId, new List<DeviceDataModel>());
                    sortedData[d.DeviceId].Add(d);
                }
                else
                {
                    sortedData[d.DeviceId].Add(d);
                }
                deviceIds.Add(d.DeviceId);
            }
            //TODO: Uniquify
            deviceIds = deviceIds.Distinct().ToList();
            Dictionary<String, String> operatorName = new Dictionary<string, string>();
            operatorName.Add("equal", "=");
            operatorName.Add("greater", ">");
            operatorName.Add("less", "<");
            foreach (String did in deviceIds)
            {
                List<ThresholdModel> thresholdDic = _thresholdDao.GetByDeviceId(did);
                foreach (DeviceDataModel data in sortedData[did])
                {
                    var query = thresholdDic.AsQueryable()
                        .Where(t => t.IndexId == data.IndexId)
                        .FirstOrDefault();
                    String op = query.Operator;
                    double threshold = query.ThresholdValue;
                    String svty = this._severityDao.GetById(int.Parse(query.Severity)).SeverityName;

                    Boolean abnormal = false;

                    if (op == "equal")
                    {
                        if (double.Parse(data.IndexValue) - threshold < 0.0001)
                        {
                            abnormal = true;
                        }
                    }
                    else if (op == "less")
                    {
                        if (double.Parse(data.IndexValue) <= threshold)
                        {
                            abnormal = true;
                        }
                    }
                    else if (op == "greater")
                    {
                        if (double.Parse(data.IndexValue) >= threshold)
                        {
                            abnormal = true;
                        }
                    }

                    if (abnormal == true)
                    {
                        AlarmInfoModel alarmInfo = new AlarmInfoModel();
                        alarmInfo.AlarmInfo = query.Description;
                        alarmInfo.DeviceId = data.DeviceId;
                        alarmInfo.IndexId = data.IndexId;
                        alarmInfo.IndexName = data.IndexName;
                        alarmInfo.IndexValue = data.IndexValue;
                        alarmInfo.ThresholdValue = operatorName[op] + threshold.ToString();
                        alarmInfo.Timestamp = DateTime.Now;
                        alarmInfo.Severity = svty;

                        _alarmInfoDao.Create(alarmInfo);
                    }
                }
            }
        }
    }
}