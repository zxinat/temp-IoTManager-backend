using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IDeviceBus _deviceBus;
        private readonly IStateTypeDao _stateTypeDao;
        private readonly IFieldBus _fieldBus;
        private readonly IFieldDao _fieldDao;
        private readonly ILogger _logger;
        private readonly IDeviceDailyOnlineTimeDao _deviceDailyOnlineTimeDao;

        public AlarmInfoJob(IDeviceDataDao deviceDataDao, 
            IAlarmInfoDao alarmInfoDao, 
            IThresholdDao thresholdDao, 
            ISeverityDao severityDao, 
            IDeviceDao deviceDao,
            IDeviceBus deviceBus,
            IStateTypeDao stateTypeDao,
            IFieldBus fieldBus,
            IFieldDao fieldDao,
            IDeviceDailyOnlineTimeDao deviceDailyOnlineTimeDao,
            ILogger<AlarmInfoJob> logger)
        {
            this._deviceDataDao = deviceDataDao;
            this._alarmInfoDao = alarmInfoDao;
            this._thresholdDao = thresholdDao;
            this._severityDao = severityDao;
            this._deviceDao = deviceDao;
            this._deviceBus = deviceBus;
            this._stateTypeDao = stateTypeDao;
            this._fieldBus = fieldBus;
            this._fieldDao = fieldDao;
            this._logger = logger;
            this._deviceDailyOnlineTimeDao = deviceDailyOnlineTimeDao;
        }
        /*定时器任务：（1）判断设备属性值是否超出阈值；（2）判断设备是否离线*/
        [Invoke(Begin = "2020-4-26 11:48", Interval = 1000 * 60, SkipWhileExecuting = true)]
        public void Run()
        {
            
            
            /*
            this._logger.LogInformation("AlarmInfoJob Run ...");
            List<DeviceModel> devices = this._deviceDao.Get("all");
            //将所有设备置为在线状态
            foreach (DeviceModel device in devices)
            {
                this._deviceDao.SetDeviceOnlineStatus(device.HardwareDeviceId, "yes");
            }
            //获取设备数据中"IsScam"="false"的所有数据，并置为"true"
            List<DeviceDataModel> dataNotInspected = _deviceDataDao.GetNotInspected();
            Dictionary<String, List<DeviceDataModel>> sortedData = new Dictionary<string, List<DeviceDataModel>>();
            Dictionary<String, List<String>> fieldMap = new Dictionary<string, List<string>>();
            List<String> deviceIds = new List<string>();
            //将deviceId相同的数据整合成sortedData,key存放deviceId,value存放相应的数据列表
            //将deviceId相同的属性Id整合成fieldMap,key存放deviceId,value存放属性Id的列表
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

            //Automatically add fields from device data
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
                this._deviceDao.UpdateLastConnectionTimeByDeviceId(did);
            }

            foreach (DeviceModel d in devices)
            {
                DeviceTypeModel tmpDeviceType = this._stateTypeDao.GetDeviceTypeByName(d.DeviceType);
                Double tmpOfflineTime = tmpDeviceType.OfflineTime;
                TimeSpan passTime = DateTime.Now - d.LastConnectionTime.ToLocalTime();
                if (passTime.TotalMinutes > tmpOfflineTime)
                {
                    this._deviceDao.SetDeviceOnlineStatus(d.HardwareDeviceId, "no");
                }
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
            }*/
            /* zxin-告警信息处理和离线判断方法的理解：
             * 告警信息处理：告警判断频率 1min
             * 1、从threshold表中获取所有告警规则
             * 2、按deviceId、MonitoringId获取前1min的数据，判断是否超过阈值，超过阈值的则向alarmInfo集合中插入告警信息
             * 设备离线判断：频率 1min
             * 1、从MySQL获取所有注册设备信息以及设备类型数据
             * 2、根据deviceId从MongoDB中获取最新一条数据，如果与当前时间差值大于超时告警时间则为离线状态（设备状态更新）
             */
            _logger.LogInformation("AlarmInfoJob Run...");
            _logger.LogInformation("告警判断...");
            /*告警判断*/
            //获取所有告警规则，插入告警信息到MongoDB
            List<ThresholdModel> alarmRules = this._thresholdDao.Get("all");
            foreach(var rule in alarmRules )
            {
                //获取具有告警规则的最新60秒的设备数据
                List<DeviceDataModel> deviceDatas = this._deviceDataDao.ListNewData(rule.DeviceId,60, rule.IndexId);
                //var isAlarmInfo=deviceDatas.AsQueryable()
                //    .Where(dd=>dd.IndexValue>rule.ThresholdValue)
                foreach(var dd in deviceDatas)
                {
                    AlarmInfoModel alarmInfo = AlarmInfoGenerator(dd, rule);
                    if(alarmInfo!=null)
                    {
                        _alarmInfoDao.Create(alarmInfo);
                    }
                    
                }
            }
            

            _logger.LogInformation("设备离线判断...");
            /*设备离线判断*/
            List<DeviceModel> devices = this._deviceDao.Get("all");
            List<DeviceTypeModel> deviceTypes = this._stateTypeDao.ListAllDeviceType();
            Dictionary<String, List<String>> fieldMap = new Dictionary<string, List<string>>();
            foreach (var device in devices)
            {
                //获取超时告警时间（分钟）
                double offlinetime = deviceTypes.AsQueryable().Where(dt => dt.DeviceTypeName == device.DeviceType).FirstOrDefault().OfflineTime;
                DateTime date = DateTime.Now - TimeSpan.FromMinutes(offlinetime);
                //获取最新一条设备数据
                DeviceDataModel deviceData = this._deviceDataDao.GetByDeviceName(device.DeviceName, 1).FirstOrDefault();
                if(deviceData!=null)
                {
                    if(deviceData.Timestamp>=date)
                    {
                        this._deviceDao.SetDeviceOnlineStatus(device.HardwareDeviceId, "yes");//设备在线
                    }
                    else
                    {
                        this._deviceDao.SetDeviceOnlineStatus(device.HardwareDeviceId, "no");
                    }
                }
                else
                {
                    this._deviceDao.SetDeviceOnlineStatus(device.HardwareDeviceId, "no");
                }

                /* 更新设备属性：
                 * 1、列出MySQL中现有属性
                 * 2、获取MongoDB中最新数据中的属性
                 * 3、比对并创建新属性
                 */
                List<string> existedFieldIds = this._fieldDao.ListFieldIdsByDeviceId(device.HardwareDeviceId);
                List<DeviceDataModel> deviceDatas= this._deviceDataDao.ListNewData(device.HardwareDeviceId, 60);
                foreach(var dd in deviceDatas)
                {
                    if(!existedFieldIds.Contains(dd.IndexId))
                    {
                        FieldModel field = new FieldModel
                        {
                            FieldId = dd.IndexId,
                            FieldName = dd.IndexName,
                            Device = dd.DeviceName
                        };
                        this._fieldDao.Create(field);
                    }
                }
                /*更新设备的总告警次数*/
                //获取设备当前总的告警次数
                //int totalInfo = _alarmInfoDao.GetDeviceAffiliateAlarmInfoNumber(device.HardwareDeviceId);
                //int totalInfo = 0;
                //更新MySQL中的设备的告警总次数
                //_deviceBus.UpdateTotalAlarmInfo(device.HardwareDeviceId, totalInfo);
            }
        }
        
        public AlarmInfoModel AlarmInfoGenerator(DeviceDataModel deviceData, ThresholdModel threshold)
        {
            AlarmInfoModel alarmInfo = new AlarmInfoModel
            {
                AlarmInfo = threshold.Description,
                DeviceId = deviceData.DeviceId,
                IndexId = deviceData.IndexId,
                IndexName = deviceData.IndexName,
                IndexValue = deviceData.IndexValue,
                ThresholdValue = threshold.ThresholdValue,
                Timestamp = DateTime.Now,
                Severity = threshold.Severity,
                Processed = "No"
            };
            if(threshold.Operator== "equal")
            {
                if(deviceData.IndexValue!=threshold.ThresholdValue)
                {
                    return alarmInfo;
                }
            }
            else if(threshold.Operator=="less")
            {
                if(deviceData.IndexValue>threshold.ThresholdValue)
                {
                    return alarmInfo;
                }
            }
            else
            {
                if(deviceData.IndexValue<threshold.ThresholdValue)
                {
                    return alarmInfo;
                }
            }
            return null;
        }
        public void ReportJob()
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
                double totalDays = deviceData != null ? (nowadays - new DateTime(deviceData.Timestamp.Year, deviceData.Timestamp.Month, deviceData.Timestamp.Day)).TotalDays
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