using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
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
        private readonly ILogger _logger;

        public DeviceDataBus(IDeviceDataDao deviceDataDao, 
            IAlarmInfoDao alarmInfoDao, 
            ILogger<DeviceDataBus> logger, 
            IDeviceDao deviceDao, 
            IWorkshopDao workshopDao)
        {
            this._deviceDataDao = deviceDataDao;
            this._alarmInfoDao = alarmInfoDao;
            this._logger = logger;
            this._deviceDao = deviceDao;
            this._workshopDao = workshopDao;
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

        public object GetDayAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime)
        {
            return this._deviceDataDao.GetDayAggregateData(deviceId, indexId, startTime, endTime);
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
    }
}