using System;
using System.Collections.Generic;
using System.Linq;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
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
        private readonly IDeviceDao _deviceDao;
        private readonly IFieldBus _fieldBus;
        private readonly ILogger _logger;

        public AlarmInfoJob(IDeviceDataDao deviceDataDao, 
            IAlarmInfoDao alarmInfoDao, 
            IThresholdDao thresholdDao, 
            ISeverityDao severityDao, 
            IDeviceDao deviceDao,
            IFieldBus fieldBus,
            ILogger<AlarmInfoJob> logger)
        {
            this._deviceDataDao = deviceDataDao;
            this._alarmInfoDao = alarmInfoDao;
            this._thresholdDao = thresholdDao;
            this._severityDao = severityDao;
            this._deviceDao = deviceDao;
            this._fieldBus = fieldBus;
            this._logger = logger;
        }

        [Invoke(Begin = "2019-6-16 16:20", Interval = 1000 * 60, SkipWhileExecuting = true)]
        public void Run()
        {
            
            List<DeviceModel> devices = this._deviceDao.Get("all");
            foreach (DeviceModel device in devices)
            {
                this._deviceDao.SetDeviceOnlineStatus(device.HardwareDeviceId, "no");
            }
            
            List<DeviceDataModel> dataNotInspected = _deviceDataDao.GetNotInspected();
            Dictionary<String, List<DeviceDataModel>> sortedData = new Dictionary<string, List<DeviceDataModel>>();
            Dictionary<String, List<String>> fieldMap = new Dictionary<string, List<string>>();
            List<String> deviceIds = new List<string>();
            System.Console.WriteLine(dataNotInspected.Count.ToString());
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

                if (!fieldMap.ContainsKey(d.DeviceId))
                {
                    fieldMap.Add(d.DeviceId, new List<string>());
                    fieldMap[d.DeviceId].Add(d.IndexId);
                }
                else
                {
                    fieldMap[d.DeviceId].Add(d.IndexId);
                }
                
                deviceIds.Add(d.DeviceId);
            }

            foreach (String did in fieldMap.Keys)
            {
                List<FieldSerializer> affiliateFields = _fieldBus.GetAffiliateFields(did);
                List<String> affiliateFieldsId = new List<string>();
                foreach (var field in affiliateFields)
                {
                    affiliateFieldsId.Add(field.fieldId);
                }

                foreach (String fid in fieldMap[did])
                {
                    if (!affiliateFieldsId.Contains(fid))
                    {
                        FieldSerializer tmpField = new FieldSerializer();
                        tmpField.fieldName = fid;
                        tmpField.fieldId = fid;
                        DeviceModel tmpDevice = this._deviceDao.GetByDeviceId(did);
                        tmpField.device = tmpDevice.DeviceName;
                        this._fieldBus.CreateNewField(tmpField);
                    }
                }
            }
            
            deviceIds = deviceIds.Distinct().ToList();

            foreach (String did in deviceIds)
            {
                this._deviceDao.SetDeviceOnlineStatus(did, "yes");
            }
            
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
                        .ToList();
                    foreach (var th in query)
                    {
                        String op = th.Operator;
                        double threshold = th.ThresholdValue;

                        Boolean abnormal = false;

                        if (op == "equal")
                        {
                            if (data.IndexValue - threshold < 0.0001)
                            {
                                abnormal = true;
                            }
                        }
                        else if (op == "less")
                        {
                            if (data.IndexValue <= threshold)
                            {
                                abnormal = true;
                            }
                        }
                        else if (op == "greater")
                        {
                            if (data.IndexValue >= threshold)
                            {
                                abnormal = true;
                            }
                        }

                        if (abnormal == true)
                        {
                            AlarmInfoModel alarmInfo = new AlarmInfoModel();
                            alarmInfo.AlarmInfo = th.Description;
                            alarmInfo.DeviceId = data.DeviceId;
                            alarmInfo.IndexId = data.IndexId;
                            alarmInfo.IndexName = data.IndexName;
                            alarmInfo.IndexValue = data.IndexValue;
                            alarmInfo.ThresholdValue = threshold;
                            alarmInfo.Timestamp = DateTime.Now;
                            alarmInfo.Severity = th.Severity;
                            alarmInfo.Processed = "No";

                            _alarmInfoDao.Create(alarmInfo);
                        }
                    }
                }
            }
        }
    }
}