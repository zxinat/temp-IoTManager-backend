using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;

namespace IoTManager.Core
{
    public sealed class DeviceDataBus: IDeviceDataBus
    {
        private readonly IDeviceDataDao _deviceDataDao;
        private readonly IAlarmInfoDao _alarmInfoDao;
        private readonly IDeviceDao _deviceDao;
        private readonly IWorkshopDao _workshopDao;
        private readonly IStateTypeDao _stateTypeDao;
        private readonly ILogger _logger;

        public DeviceDataBus(IDeviceDataDao deviceDataDao, 
            IAlarmInfoDao alarmInfoDao, 
            ILogger<DeviceDataBus> logger, 
            IDeviceDao deviceDao, 
            IWorkshopDao workshopDao,
            IStateTypeDao stateTypeDao)
        {
            this._deviceDataDao = deviceDataDao;
            this._alarmInfoDao = alarmInfoDao;
            this._logger = logger;
            this._deviceDao = deviceDao;
            this._workshopDao = workshopDao;
            this._stateTypeDao = stateTypeDao;
        }

        public List<DeviceDataSerializer> GetAllDeviceData(String searchType, String deviceId = "all", int page = 1, String sortColumn = "Id", String order = "asc")
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            List<DeviceDataModel> deviceData = this._deviceDataDao.Get(searchType, deviceId, offset, limit, sortColumn, order);
            List<DeviceDataSerializer> result = new List<DeviceDataSerializer>();
            foreach (DeviceDataModel dd in deviceData)
            {
                result.Add(new DeviceDataSerializer(dd));
            }
            return result;
        }

        public DeviceDataSerializer GetDeviceDataById(String Id)
        {
            DeviceDataModel deviceData = this._deviceDataDao.GetById(Id);
            DeviceDataSerializer result = new DeviceDataSerializer(deviceData);
            return result;
        }

        public List<DeviceDataSerializer> GetDeviceDataByDeviceId(String DeviceId)
        {
            List<DeviceDataModel> deviceData = this._deviceDataDao.GetByDeviceId(DeviceId);
            List<DeviceDataSerializer> result = new List<DeviceDataSerializer>();
            foreach (DeviceDataModel dd in deviceData)
            {
                result.Add(new DeviceDataSerializer(dd));
            }
            return result;
        }

        public Object GetLineChartData(String deviceId, String indexId)
        {
            return this._deviceDataDao.GetLineChartData(deviceId, indexId);
        }

        public int GetDeviceDataAmount()
        {
            return this._deviceDataDao.GetDeviceDataAmount();
        }

        public object GetDeviceStatusById(int id, DateTime sTime, DateTime eTime)
        {
            return this._deviceDataDao.GetDeviceStatusById(id, sTime, eTime);
        }

        public long GetDeviceDataNumber(String searchType, String deviceId = "all")
        {
            return this._deviceDataDao.GetDeviceDataNumber(searchType, deviceId);
        }

        public String DeleteDeviceData(String id)
        {
            return this._deviceDataDao.Delete(id);
        }

        public int BatchDeleteDeviceData(List<String> ids)
        {
            return this._deviceDataDao.BatchDelete(ids);
        }

        public String UpdateDeviceData(String id, DeviceDataSerializer deviceDataSerializer)
        {
            DeviceDataModel deviceDataModel = new DeviceDataModel();
            deviceDataModel.Id = deviceDataSerializer.id;
            deviceDataModel.DeviceId = deviceDataSerializer.deviceId;
            deviceDataModel.IndexName = deviceDataSerializer.indexName;
            deviceDataModel.IndexId = deviceDataSerializer.indexId;
            deviceDataModel.IndexUnit = deviceDataSerializer.indexUnit;
            deviceDataModel.IndexType = deviceDataSerializer.indexType;
            deviceDataModel.IndexValue = deviceDataSerializer.indexValue;
            return this._deviceDataDao.Update(id, deviceDataModel);
        }

        public object GetDayAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime, String scale)
        {
            return this._deviceDataDao.GetDayAggregateData(deviceId, indexId, startTime, endTime, scale);
        }

        public object GetMultipleLineChartData(String deviceId, List<String> fields)
        {
            List<object> data = new List<object>();
            foreach (String f in fields)
            {
                data.Add(this._deviceDataDao.GetLineChartData(deviceId, f));
            }

            return data;
        }

        public object GetDashboardDeviceStatus()
        {
            var query = this._alarmInfoDao.Get("all");
            var info = query.AsQueryable()
                .Where(ai => ai.Severity == "Info").ToList();
            List<String> infoDevices = new List<string>();
            foreach (var a in info)
            {
                if (!infoDevices.Contains(a.DeviceId))
                {
                    infoDevices.Add(a.DeviceId);
                }
            }

            var warning = query.AsQueryable()
                .Where(ai => ai.Severity == "Warning").ToList();
            List<String> warningDevices = new List<string>();
            foreach (var a in warning)
            {
                if (!warningDevices.Contains(a.DeviceId))
                {
                    warningDevices.Add(a.DeviceId);
                }
            }

            var critical = query.AsQueryable()
                .Where(ai => ai.Severity == "Critical").ToList();
            List<String> criticalDevices = new List<string>();
            foreach (var a in critical)
            {
                if (!criticalDevices.Contains(a.DeviceId))
                {
                    criticalDevices.Add(a.DeviceId);
                }
            }
            return new
            {
                info = infoDevices.Count,
                warning = warningDevices.Count,
                critical = criticalDevices.Count
            };
        }

        public int GetDeviceAffiliateData(String deviceId)
        {
            return this._deviceDataDao.GetDeviceAffiliateData(deviceId);
        }

        public object GetReportByRegion(String factoryName, DateTime startTime, DateTime endTime)
        {
            List<WorkshopModel> affiliateWorkshop = this._workshopDao.GetAffiliateWorkshop(factoryName);
            List<String> xAxis = new List<string>();
            List<Double> averageOnlineTime = new List<Double>();
            List<int> alarmTimes = new List<int>();
            List<int> deviceAmount = new List<int>();
            foreach (WorkshopModel w in affiliateWorkshop)
            {
                xAxis.Add(w.WorkshopName);
                
                List<DeviceModel> allDevices = this._deviceDao.Get("all");
                List<DeviceModel> relatedDevices = allDevices.AsQueryable()
                    .Where(d => d.Workshop == w.WorkshopName)
                    .ToList();
                deviceAmount.Add(relatedDevices.Count);
                
                List<String> relatedDevicesId = new List<string>();
                foreach (DeviceModel d in relatedDevices)
                {
                    relatedDevicesId.Add(d.HardwareDeviceId);
                }

                List<AlarmInfoModel> allAlarmInfo = this._alarmInfoDao.Get("all");
                List<AlarmInfoModel> relatedAlarmInfo = allAlarmInfo.AsQueryable()
                    .Where(ai => relatedDevicesId.Contains(ai.DeviceId) && ai.Timestamp >= startTime && ai.Timestamp <= endTime)
                    .ToList();
                alarmTimes.Add(relatedAlarmInfo.Count);
                
                TimeSpan t = TimeSpan.Zero;
                foreach (String did in relatedDevicesId)
                {
                    List<DeviceDataModel> allDeviceData = this._deviceDataDao.Get("all");
                    List<DeviceDataModel> relatedData = allDeviceData.AsQueryable()
                        .Where(dd => dd.DeviceId == did && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                        .OrderBy(dd => dd.Timestamp)
                        .ToList();
                    if (relatedData.Count > 0)
                    {
                        var tmpTime = relatedData.Last().Timestamp - relatedData.First().Timestamp;
                        t += tmpTime;
                    }
                }
                averageOnlineTime.Add(t.TotalMinutes / relatedDevices.Count);
            }
            
            List<object> result = new List<object>();
            result.Add(new {name = "平均在线时间", data = averageOnlineTime, type = "bar", barWidth = 20});
            result.Add(new {name = "告警次数", data = alarmTimes, type = "bar", barWidth = 20});
            result.Add(new {name = "设备数量", data = deviceAmount, type = "bar", barWidth = 20});
            return new
            {
                xAxis = xAxis,
                series = result
            };
        }

        public object GetReportByTime(DateTime startTime, DateTime endTime)
        {
            List<DeviceModel> devices = this._deviceDao.Get("all");
            List<String> xAxis = new List<string>();
            List<Double> averageOnlineTime = new List<Double>();
            List<int> alarmTimes = new List<int>();
            List<int> deviceAmount = new List<int>();

            List<DeviceDataModel> deviceData = this._deviceDataDao.Get("all");
            List<DeviceDataModel> filteredDeviceData = deviceData.AsQueryable()
                .Where(dd => dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                .OrderBy(dd => dd.Timestamp)
                .ToList();
            List<int> years = new List<int>();
            foreach (DeviceDataModel d in filteredDeviceData)
            {
                if (!years.Contains(d.Timestamp.Year))
                {
                    years.Add(d.Timestamp.Year);
                }
            }
            
            years.Sort();

            foreach (int year in years)
            {
                xAxis.Add(year.ToString());
                
                deviceAmount.Add(devices.Count);
                
                List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.Get("all");
                List<AlarmInfoModel> selectedAlarmInfo = alarmInfos.AsQueryable()
                    .Where(ai => ai.Timestamp.Year == year)
                    .ToList();
                alarmTimes.Add(selectedAlarmInfo.Count);
                
                
                List<DeviceDataModel> deviceDataByYear = filteredDeviceData.AsQueryable()
                    .Where(dd => dd.Timestamp.Year == year)
                    .ToList();
                
                TimeSpan t = TimeSpan.Zero;
                foreach (DeviceModel d in devices)
                {
                    List<DeviceDataModel> relatedData = deviceDataByYear.AsQueryable()
                        .Where(dd => dd.DeviceId == d.HardwareDeviceId)
                        .OrderBy(dd => dd.Timestamp)
                        .ToList();
                    if (relatedData.Count > 0)
                    {
                        var tmpTime = relatedData.Last().Timestamp - relatedData.First().Timestamp;
                        t += tmpTime;
                    }
                }
                averageOnlineTime.Add(t.TotalMinutes / devices.Count);
            }
            
            List<object> result = new List<object>();
            result.Add(new {name = "平均在线时间", data = averageOnlineTime, type = "bar", barWidth = 20});
            result.Add(new {name = "告警次数", data = alarmTimes, type = "bar", barWidth = 20});
            result.Add(new {name = "设备数量", data = deviceAmount, type = "bar", barWidth = 20});

            return new
            {
                xAxis = xAxis,
                series = result
            };
        }

        public object GetReportByType(DateTime startTime, DateTime endTime)
        {
            List<String> deviceTypes = this._stateTypeDao.GetDeviceType();
            
            List<String> xAxis = new List<string>();
            List<Double> averageOnlineTime = new List<Double>();
            List<object> alarmTimes = new List<object>();
            List<object> deviceAmount = new List<object>();
            
            foreach (String deviceType in deviceTypes)
            {
                xAxis.Add(deviceType);
                
                List<DeviceModel> relatedDevices = this._deviceDao.GetByDeviceType(deviceType);
                List<String> relatedDevicesId = new List<string>();
                foreach (DeviceModel dm in relatedDevices)
                {
                    relatedDevicesId.Add(dm.HardwareDeviceId);
                }

                if (relatedDevices.Count > 0)
                {
                    deviceAmount.Add(new 
                    {
                        value = relatedDevices.Count,
                        name = deviceType
                    });
                }

                List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.Get("all");
                List<AlarmInfoModel> relatedAlarmInfos = alarmInfos.AsQueryable()
                    .Where(ai =>
                        relatedDevicesId.Contains(ai.DeviceId) && ai.Timestamp >= startTime && ai.Timestamp <= endTime)
                    .ToList();
                if (relatedAlarmInfos.Count > 0)
                {
                    alarmTimes.Add(new
                    {
                        value = relatedAlarmInfos.Count,
                        name = deviceType
                    });
                }

                TimeSpan t = TimeSpan.Zero;
                foreach (DeviceModel dm in relatedDevices)
                {
                    List<DeviceDataModel> deviceData = this._deviceDataDao.Get("all");
                    List<DeviceDataModel> relatedData = deviceData.AsQueryable()
                        .Where(dd =>
                            dd.DeviceId == dm.HardwareDeviceId && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                        .OrderBy(dd => dd.Timestamp)
                        .ToList();
                    if (relatedData.Count > 0)
                    {
                        var tmpTime = relatedData.Last().Timestamp - relatedData.First().Timestamp;
                        t += tmpTime;
                    }
                }
                averageOnlineTime.Add(t.TotalMinutes / relatedDevices.Count);
            }
            List<object> lineChartSeries = new List<object>();
            lineChartSeries.Add(new {name = "平均在线时间", data = averageOnlineTime, type = "bar", barWidth = 20});
            List<object> pieChart1Series = new List<object>();
            System.Console.WriteLine("Alarm Times:  " + alarmTimes.Count.ToString());
            if (alarmTimes.Count == 0)
            {
                alarmTimes.Add(new {value = 0, name = "无数据"});
            }
            pieChart1Series.Add(alarmTimes);
            List<object> pieChart2Series = new List<object>();
            if (deviceAmount.Count == 0)
            {
                deviceAmount.Add(new {value = 0, name = "无数据"});
            }
            pieChart2Series.Add(deviceAmount);

            return new
            {
                xAxis = xAxis,
                lineChartSeries = lineChartSeries,
                pieChart1Series = alarmTimes,
                pieChart2Series = deviceAmount
            };
        }

        public object GetReportByTag(DateTime startTime, DateTime endTime)
        {
            List<String> tags = this._deviceDao.GetAllTag();
            List<DeviceDataModel> deviceData = this._deviceDataDao.Get("all");
            
            List<String> xAxis = new List<string>();
            List<Double> averageOnlineTime = new List<Double>();
            List<int> alarmTimes = new List<int>();
            List<int> deviceAmount = new List<int>();

            foreach (String tag in tags)
            {
                xAxis.Add(tag);

                List<DeviceModel> relatedDevices = this._deviceDao.GetDeviceByTag(tag);
                deviceAmount.Add(relatedDevices.Count);
                
                List<String> relatedDevicesId = new List<string>();
                foreach (DeviceModel d in relatedDevices)
                {
                    relatedDevicesId.Add(d.HardwareDeviceId);
                }

                List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.Get("all");
                List<AlarmInfoModel> relatedAlarmInfos = alarmInfos.AsQueryable()
                    .Where(ai =>
                        relatedDevicesId.Contains(ai.DeviceId) && ai.Timestamp >= startTime && ai.Timestamp <= endTime)
                    .ToList();
                alarmTimes.Add(relatedAlarmInfos.Count);

                TimeSpan t = TimeSpan.Zero;
                foreach (DeviceModel device in relatedDevices)
                {
                    List<DeviceDataModel> relatedDeviceData = deviceData.AsQueryable()
                        .Where(dd =>
                            dd.DeviceId == device.HardwareDeviceId && dd.Timestamp >= startTime &&
                            dd.Timestamp <= endTime)
                        .OrderBy(dd => dd.Timestamp)
                        .ToList();
                    if (relatedDeviceData.Count > 0)
                    {
                        TimeSpan tmpTime = relatedDeviceData.Last().Timestamp - relatedDeviceData.First().Timestamp;
                        t += tmpTime;
                    }
                }
                averageOnlineTime.Add(t.TotalMinutes / relatedDevices.Count);
            }
            
            List<object> result = new List<object>();
            result.Add(new {name = "平均在线时间", data = averageOnlineTime, type = "bar", barWidth = 20});
            result.Add(new {name = "告警次数", data = alarmTimes, type = "bar", barWidth = 20});
            result.Add(new {name = "设备数量", data = deviceAmount, type = "bar", barWidth = 20});
            
            return new
            {
                xAxis = xAxis,
                series = result
            };
        }

        public int GetFieldAffiliateData(String fieldId)
        {
            return this._deviceDataDao.GetFieldAffiliateData(fieldId);
        }
    }
}