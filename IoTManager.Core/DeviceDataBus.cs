/*
 * DeviceDataBus.cs
 * 设备数据管理的业务逻辑层
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Google.Protobuf.WellKnownTypes;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
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
        private readonly IDeviceDailyOnlineTimeDao _deviceDailyOnlineTimeDao;
        private readonly IThresholdDao _thresholdDao;
        private readonly IFieldDao _fieldDao;
        private readonly ILogger _logger;

        /*
         * 构造函数
         * 需要注入DeviceDataDao, AlarmInfoDao, DeviceDao, WorkshopDao, StateTypeDao
         */
        public DeviceDataBus(IDeviceDataDao deviceDataDao, 
            IAlarmInfoDao alarmInfoDao, 
            ILogger<DeviceDataBus> logger, 
            IDeviceDao deviceDao, 
            IWorkshopDao workshopDao,
            IStateTypeDao stateTypeDao,
            IDeviceDailyOnlineTimeDao deviceDailyOnlineTimeDao,
            IThresholdDao thresholdDao,
            IFieldDao fieldDao
            )
        {
            this._deviceDataDao = deviceDataDao;
            this._alarmInfoDao = alarmInfoDao;
            this._logger = logger;
            this._deviceDao = deviceDao;
            this._workshopDao = workshopDao;
            this._stateTypeDao = stateTypeDao;
            this._deviceDailyOnlineTimeDao = deviceDailyOnlineTimeDao;
            this._thresholdDao = thresholdDao;
            this._fieldDao = fieldDao;
        }

        /*
         * 获取所有设备数据，带分页功能
         *
         * 输入：
         * 搜索类型
         * 设备编号
         * 页码
         * 排序列
         * 升序或降序
         *
         * 输出：
         * 设备数据列表
         */
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

        /*
         * 根据数据ID（数据库ID）获取数据
         *
         * 输入：
         * ID
         *
         * 输出：
         * 该ID的设备数据
         */
        public DeviceDataSerializer GetDeviceDataById(String Id)
        {
            DeviceDataModel deviceData = this._deviceDataDao.GetById(Id);
            DeviceDataSerializer result = new DeviceDataSerializer(deviceData);
            return result;
        }

        /*
         * 根据设备编号获取下属所有设备数据
         *
         * 输入：
         * 设备编号
         *
         * 输出：
         * 该设备下的所有设备数据
         */
        public List<DeviceDataSerializer> GetDeviceDataByDeviceId20(String DeviceId)
        {
            List<DeviceDataModel> deviceData = this._deviceDataDao.GetByDeviceId20(DeviceId);
            List<DeviceDataSerializer> result = new List<DeviceDataSerializer>();
            foreach (DeviceDataModel dd in deviceData)
            {
                result.Add(new DeviceDataSerializer(dd));
            }
            return result;
        }

        /*
         * 根据设备编号和属性获取仪表板折线图格式数据
         *
         * 输入：
         * 设备编号
         * 设备属性
         *
         * 输出：
         * 折线图格式数据
         */
        public Object GetLineChartData(String deviceId, String indexId)
        {
            return this._deviceDataDao.GetLineChartData(deviceId, indexId);
        }

        /*
         * 获取设备数据总数
         */
        public int GetDeviceDataAmount()
        {
            return this._deviceDataDao.GetDeviceDataAmount();
        }

        /*
         * 根据获取设备概况，此数据用于监控配置页面显示设备概况
         */
        public object GetDeviceStatusById(int id, DateTime sTime, DateTime eTime)
        {
            return this._deviceDataDao.GetDeviceStatusById(id, sTime, eTime);
        }

        /*
         * 获取特定分页条件下的设备数据总数
         *
         * 输入：
         * 搜索类型
         * 设备编号
         *
         * 输出：
         * 设备数据总数
         */
        public long GetDeviceDataNumber(String searchType, String deviceId = "all")
        {
            return this._deviceDataDao.GetDeviceDataNumber(searchType, deviceId);
        }

        /*
         * 根据设备数据ID（数据库ID）删除数据
         *
         * 输入：
         * ID
         *
         * 输出：
         * 无
         */
        public String DeleteDeviceData(String id)
        {
            return this._deviceDataDao.Delete(id);
        }

        /*
         * 批量删除
         */
        public int BatchDeleteDeviceData(List<String> ids)
        {
            return this._deviceDataDao.BatchDelete(ids);
        }

        /*
         * 更新设备数据
         */
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

        /*
         * 根据设备编号和属性获取统计信息，此数据主要用于监控配置页面设备数据导出
         *
         * 输入：
         * 设备编号
         * 属性编号
         * 起始时间
         * 结束时间
         * 统计粒度（小时、日、月）
         *
         * 输出：
         * 统计信息
         */
        public object GetDayAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime, String scale)
        {
            return this._deviceDataDao.GetDayAggregateData(deviceId, indexId, startTime, endTime, scale);
        }

        /*
         * 根据设备编号和属性获取多组仪表板折线图格式数据
         *
         * 输入：
         * 设备编号
         * 设备属性
         *
         * 输出：
         * 折线图格式数据（多组）
         */
        public object GetMultipleLineChartData(String deviceId, List<String> fields)
        {
            List<object> data = new List<object>();
            foreach (String f in fields)
            {
                data.Add(this._deviceDataDao.GetLineChartData(deviceId, f));
            }

            return data;
        }

        /*
         * 获取仪表板告警信息
         */
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

        /*
         * 根据设备编号获取设备数据数量
         *
         * 输入：
         * 设备编号
         *
         * 输出：
         * 该设备下的设备数据数量
         */
        public int GetDeviceAffiliateData(String deviceId)
        {
            return this._deviceDataDao.GetDeviceAffiliateData(deviceId);
        }

        /*
         * 地域纬度报表
         */
        public object GetReportByRegion(String factoryName, DateTime startTime, DateTime endTime)
        {
            List<WorkshopModel> affiliateWorkshop = this._workshopDao.GetAffiliateWorkshop(factoryName);
            List<String> xAxis = new List<string>();
            List<Double> averageOnlineTime = new List<Double>();
            List<int> alarmTimes = new List<int>();
            List<int> deviceAmount = new List<int>();
            
            List<DeviceModel> allDevices = this._deviceDao.Get("all");
            List<AlarmInfoModel> allAlarmInfo = this._alarmInfoDao.Get("all");
            
            /*new*/
            List<DeviceDailyOnlineTimeModel> dailyOnlineTime =
                this._deviceDailyOnlineTimeDao.GetDeviceOnlineTimeByTime(startTime, endTime);
            /*new*/
            
            foreach (WorkshopModel w in affiliateWorkshop)
            {
                xAxis.Add(w.WorkshopName);
                
                List<DeviceModel> relatedDevices = allDevices.AsQueryable()
                    .Where(d => d.Workshop == w.WorkshopName)
                    .ToList();
                deviceAmount.Add(relatedDevices.Count);
                
                List<String> relatedDevicesId = new List<String>();
                foreach (DeviceModel d in relatedDevices)
                {
                    relatedDevicesId.Add(d.HardwareDeviceId);
                }

                List<AlarmInfoModel> relatedAlarmInfo = allAlarmInfo.AsQueryable()
                    .Where(ai => relatedDevicesId.Contains(ai.DeviceId) && ai.Timestamp >= startTime && ai.Timestamp <= endTime)
                    .ToList();
                alarmTimes.Add(relatedAlarmInfo.Count);

                Double total = 0;
                List<DeviceDailyOnlineTimeModel> deviceDataByWorkshop = dailyOnlineTime.AsQueryable()
                    .Where(dot => relatedDevicesId.Contains(dot.HardwareDeviceId))
                    .ToList();
                foreach (var d in deviceDataByWorkshop)
                {
                    total += d.OnlineTime;
                }

                if (deviceDataByWorkshop.Count != 0)
                {
                    averageOnlineTime.Add(Math.Round(total / deviceDataByWorkshop.Count, 2));
                }
                else
                {
                    averageOnlineTime.Add(0);
                }

                /* old
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
                */
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

        /*
         * 时间维度报表
         */
        public object GetReportByTime(DateTime startTime, DateTime endTime)
        {
            List<DeviceModel> devices = this._deviceDao.Get("all");
            List<String> xAxis = new List<string>();
            List<Double> averageOnlineTime = new List<Double>();
            List<int> alarmTimes = new List<int>();
            List<int> deviceAmount = new List<int>();
            
            /*new*/
            List<DeviceDailyOnlineTimeModel> dailyOnlineTime =
                this._deviceDailyOnlineTimeDao.GetDeviceOnlineTimeByTime(startTime, endTime);
            //在这里使用ValueTuple存储年-月组合，Item1为年份，Item2为月份
            List<ValueTuple<int, int>> yearMonth = new List<ValueTuple<int, int>>();
            foreach (DeviceDailyOnlineTimeModel d in dailyOnlineTime)
            {
                var tmp = new ValueTuple<int, int>(d.Date.Year, d.Date.Month);
                if (!yearMonth.Contains(tmp))
                {
                    yearMonth.Add(tmp);
                }
            }
            
            yearMonth.Sort();

            foreach (var ym in yearMonth)
            {
                String t = ym.Item1.ToString() + "-" + ym.Item2.ToString();
                xAxis.Add(t);

                deviceAmount.Add(devices.Count);

                List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.Get("all");
                List<AlarmInfoModel> selectedAlarmInfo = alarmInfos.AsQueryable()
                    .Where(ai => ai.Timestamp.Year == ym.Item1 && ai.Timestamp.Month == ym.Item2)
                    .ToList();
                alarmTimes.Add(selectedAlarmInfo.Count);

                Double total = 0;
                List<DeviceDailyOnlineTimeModel> deviceDataByYearMonth = dailyOnlineTime.AsQueryable()
                    .Where(dot => dot.Date.Year == ym.Item1 && dot.Date.Month == ym.Item2)
                    .ToList();
                foreach (var d in deviceDataByYearMonth)
                {
                    total += d.OnlineTime;
                }

                if (deviceDataByYearMonth.Count != 0)
                {
                    averageOnlineTime.Add(Math.Round(total / deviceDataByYearMonth.Count, 2));
                }
                else
                {
                    averageOnlineTime.Add(0);
                }
            }
            /*new*/

            /* old
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
            */
            
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

        /*
         * 设备类型纬度报表
         */
        public object GetReportByType(DateTime startTime, DateTime endTime)
        {
            List<String> deviceTypes = this._stateTypeDao.GetDeviceType();
            List<DeviceModel> allDevices = this._deviceDao.Get("all");
            
            List<String> xAxis = new List<string>();
            List<Double> averageOnlineTime = new List<Double>();
            List<object> alarmTimes = new List<object>();
            List<object> deviceAmount = new List<object>();
            
            List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.Get("all");
            
            /*new*/
            List<DeviceDailyOnlineTimeModel> dailyOnlineTime =
                this._deviceDailyOnlineTimeDao.GetDeviceOnlineTimeByTime(startTime, endTime);
            /*new*/
            
            foreach (String deviceType in deviceTypes)
            {
                xAxis.Add(deviceType);
                
                //List<DeviceModel> relatedDevices = this._deviceDao.GetByDeviceType(deviceType);
                List<DeviceModel> relatedDevices = allDevices.AsQueryable()
                    .Where(d => d.DeviceType == deviceType)
                    .ToList();
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
                
                /*new*/
                Double total = 0;
                List<DeviceDailyOnlineTimeModel> deviceDataByType = dailyOnlineTime.AsQueryable()
                    .Where(dot => relatedDevicesId.Contains(dot.HardwareDeviceId))
                    .ToList();
                foreach (var d in deviceDataByType)
                {
                    total += d.OnlineTime;
                }

                if (deviceDataByType.Count != 0)
                {
                    averageOnlineTime.Add(Math.Round(total / deviceDataByType.Count, 2));
                }
                else
                {
                    averageOnlineTime.Add(0);
                }
                /*new*/

                /* old
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
                */
            }
            List<object> lineChartSeries = new List<object>();
            lineChartSeries.Add(new {name = "平均在线时间", data = averageOnlineTime, type = "bar", barWidth = 20});
            List<object> pieChart1Series = new List<object>();
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

        /*
         * 标签维度报表
         */
        public object GetReportByTag(DateTime startTime, DateTime endTime)
        {
            List<String> tags = this._deviceDao.GetAllTag();
            //List<DeviceDataModel> deviceData = this._deviceDataDao.Get("all");
            
            List<String> xAxis = new List<string>();
            List<Double> averageOnlineTime = new List<Double>();
            List<int> alarmTimes = new List<int>();
            List<int> deviceAmount = new List<int>();
            
            List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.Get("all");

            /*new*/
            List<DeviceDailyOnlineTimeModel> dailyOnlineTime =
                this._deviceDailyOnlineTimeDao.GetDeviceOnlineTimeByTime(startTime, endTime);
            /*new*/

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

                List<AlarmInfoModel> relatedAlarmInfos = alarmInfos.AsQueryable()
                    .Where(ai =>
                        relatedDevicesId.Contains(ai.DeviceId) && ai.Timestamp >= startTime && ai.Timestamp <= endTime)
                    .ToList();
                alarmTimes.Add(relatedAlarmInfos.Count);
                
                /*new*/
                Double total = 0;
                List<DeviceDailyOnlineTimeModel> deviceDataByTag = dailyOnlineTime.AsQueryable()
                    .Where(dot => relatedDevicesId.Contains(dot.HardwareDeviceId))
                    .ToList();
                foreach (var d in deviceDataByTag)
                {
                    total += d.OnlineTime;
                }

                if (deviceDataByTag.Count != 0)
                {
                    averageOnlineTime.Add(Math.Round(total / deviceDataByTag.Count, 2));
                }
                else
                {
                    averageOnlineTime.Add(0);
                }
                /*new*/

                /* old
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
                */
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

        /*
         * 根据属性获取下属数据数量
         *
         * 输入：
         * 属性编号
         *
         * 输出：
         * 该属性下设备数据的数量
         */
        public int GetFieldAffiliateData(String fieldId)
        {
            return this._deviceDataDao.GetFieldAffiliateData(fieldId);
        }

        /*
         *
         * 获取监控配置Device Card（设备概况）中数据的接口
         * 
         */
        public Object GetDeviceDataInDeviceCardByName(String deviceName)
        {
            //获取设备基本信息
            DeviceModel device = this._deviceDao.GetByDeviceNamePrecise(deviceName);
            
            //获取设备的数据
            List<DeviceDataModel> deviceData = this._deviceDataDao.GetByDeviceId(device.HardwareDeviceId);
            
            //将设备数据时间加入List
            List<DateTime> deviceDataTimeList = new List<DateTime>();
            foreach (var dd in deviceData)
            {
                deviceDataTimeList.Add(dd.Timestamp);
            }
            deviceDataTimeList.Sort();
            
            //获取设备的告警信息
            List<AlarmInfoModel> alarmInfo = this._alarmInfoDao.GetByDeviceId(deviceName);
            
            //将告警信息时间加入List
            List<DateTime> alarmInfoTimeList = new List<DateTime>();
            foreach (var ai in alarmInfo)
            {
                alarmInfoTimeList.Add(ai.Timestamp);
            }
            alarmInfoTimeList.Sort();
            
            //计算启动时间
            DateTime startTime = DateTime.MinValue;
            if (deviceData.Count > 0)
            {
                startTime = deviceDataTimeList[0];
            }
            
            //计算运行时间
            TimeSpan runningTime = TimeSpan.Zero;
            if (deviceData.Count > 0)
            {
                runningTime = deviceDataTimeList[deviceDataTimeList.Count - 1] - deviceDataTimeList[0];
            }
            
            //获取告警次数
            String alarmTimes = alarmInfo.Count.ToString();
            
            //获取最近报警时间
            DateTime recentAlarmTime = DateTime.MinValue;
            if (alarmInfo.Count > 0)
            {
                recentAlarmTime = alarmInfoTimeList[0];
            }
            return new
            {
                hardwareDeviceID = device.HardwareDeviceId,
                deviceName = device.DeviceName,
                deviceType = device.DeviceType,
                deviceState = device.DeviceState,
                base64Image = device.Base64Image,
                startTime = startTime == DateTime.MinValue ? "未收到数据" : startTime.ToString(Constant.getDateFormatString()),
                runningTime = runningTime == TimeSpan.Zero ? "未收到数据" : runningTime.ToString("%d") + "天" +
                                                                       runningTime.ToString("%h") + "小时" + 
                                                                       runningTime.ToString("%m") + "分钟" + 
                                                                       runningTime.ToString("%s") + "秒",
                alarmTimes = alarmTimes,
                recentAlarmTime = recentAlarmTime == 
                                  DateTime.MinValue ? "未收到数据" : recentAlarmTime.ToString(Constant.getDateFormatString())
            };
        }

        /*
         *
         * 获取监控配置Device Property（设备数据）中数据的接口
         * 
         */
        public Object GetDeviceDataInDevicePropertyByName(String deviceName)
        {
            DeviceModel device = this._deviceDao.GetByDeviceNamePrecise(deviceName);
            List<DeviceDataModel> deviceData = this._deviceDataDao.GetByDeviceId(device.HardwareDeviceId);
            List<DeviceDataModel> result = deviceData.AsQueryable()
                .OrderByDescending(dd => dd.Timestamp)
                .Take(10)
                .ToList();
            foreach (var dd in result)
            {
                dd.DeviceId = dd.Timestamp.ToString(Constant.getDateFormatString());
            }
            return result;
        }

        /*
         *
         * 获取监控配置Alarm Info（报警记录）中数据的接口
         * 
         */
        public Object GetAlarmInfoInAlarmRecordByName(String deviceName)
        {
            DeviceModel device = this._deviceDao.GetByDeviceNamePrecise(deviceName);
            List<AlarmInfoModel> alarmInfo = this._alarmInfoDao.GetByDeviceId(device.HardwareDeviceId);
            List<AlarmInfoModel> result = alarmInfo.AsQueryable()
                .OrderByDescending(ai => ai.Timestamp)
                .Take(10)
                .ToList();
            foreach (var ai in result)
            {
                ai.DeviceId = ai.Timestamp.ToString(Constant.getDateFormatString());
            }
            return result;
        }

        public Object GetRuleInDeviceAlarmingRuleByName(String deviceName)
        {
            DeviceModel device = this._deviceDao.GetByDeviceNamePrecise(deviceName);
            List<ThresholdModel> thresholdModels = this._thresholdDao.GetByDeviceId(device.HardwareDeviceId);
            List<Object> ruleResult = new List<object>();
            Dictionary<String, String> opDict = new Dictionary<string, string>();
            opDict.Add("greater", ">");
            opDict.Add("equal", "=");
            opDict.Add("less", "<");
            foreach (ThresholdModel t in thresholdModels)
            {
                ruleResult.Add(new
                {
                    name = t.RuleName,
                    description = t.Description,
                    conditionString = t.IndexId + opDict[t.Operator.ToString()] + t.ThresholdValue.ToString(),
                    severity = t.Severity
                });
            }
            return ruleResult;
        }

        public Object Get100DataInDataStatisticByName(String deviceName)
        {
            String tmpTimeSerializeStr = "MM-dd HH:mm:ss";
            
            DeviceModel device = this._deviceDao.GetByDeviceNamePrecise(deviceName);
            List<DeviceDataModel> deviceData = this._deviceDataDao.GetByDeviceId100(device.HardwareDeviceId);
            List<ValueTuple<DateTime, String, DeviceDataModel>> dataTuple = new List<(DateTime, string, DeviceDataModel)>();
            List<String> affiliateFields = new List<string>();
            List<String> xAxis = new List<string>();
            foreach (var dd in deviceData)
            {
                if (!affiliateFields.Contains(dd.IndexName))
                {
                    affiliateFields.Add(dd.IndexName);
                }
                
                DateTime tmp = dd.Timestamp;
                dd.Timestamp = new DateTime(tmp.Year, tmp.Month, tmp.Day, tmp.Hour, tmp.Minute, tmp.Second);

                String formalizedTime = dd.Timestamp.ToString(tmpTimeSerializeStr);
                if (!xAxis.Contains(formalizedTime))
                {
                    xAxis.Add(formalizedTime);
                }
                
                dataTuple.Add((dd.Timestamp, dd.IndexName, dd));
            }
            
            Dictionary<String, bool> check = new Dictionary<string, bool>();
            foreach (var f in affiliateFields)
            {
               check.Add(f, false);
            }

            Dictionary<String, List<String>> result = new Dictionary<String, List<String>>();
            foreach (var f in affiliateFields)
            {
                result.Add(f, new List<string>());
            }
            
            foreach (var t in xAxis)
            {
                foreach (var f in affiliateFields)
                {
                    check[f] = false;
                }
                
                var selectedTuple = dataTuple.AsQueryable()
                    .Where(dt => dt.Item1.ToString(tmpTimeSerializeStr) == t)
                    .ToList();

                foreach (var st in selectedTuple)
                {
                    check[st.Item2] = true;
                    result[st.Item2].Add(st.Item3.IndexValue.ToString());
                }

                foreach (var f in affiliateFields)
                {
                    if (check[f] == false)
                    {
                        result[f].Add(null);
                    }
                }
            }

            foreach (var f in result.Keys)
            {
                result[f].Reverse();
            }

            xAxis.Reverse();
            
            List<Object> seriesResult = new List<object>();
            List<String> legendResult = new List<string>();
            foreach (var f in result.Keys)
            {
                seriesResult.Add(new
                {
                    type = "line",
                    name = f,
                    data = result[f],
                    connectNulls = true
                });
                legendResult.Add(f);
            }
            
            return new
            {
                xAxis = xAxis,
                series = seriesResult,
                legend = legendResult
            };
        }

        public List<FieldModel> GetFieldByDeviceName(String deviceName)
        {
            List<FieldModel> fields = this._fieldDao.Get();
            List<FieldModel> result = fields.AsQueryable()
                .Where(f => f.Device == deviceName)
                .ToList();
            return result;
        }

        public Object GetHourAggregateDataByDeviceNameAndField(String deviceName, String fieldId, DateTime startTime, DateTime endTime)
        {
            DeviceModel device = this._deviceDao.GetByDeviceNamePrecise(deviceName);
            var result = this._deviceDataDao.GetHourAggregateData(device.HardwareDeviceId, fieldId, startTime, endTime);
            return result;
        }

        public Object GetDayAggregateDataByDeviceNameAndField(String deviceName, String fieldId, DateTime startTime, DateTime endTime)
        {
            DeviceModel device = this._deviceDao.GetByDeviceNamePrecise(deviceName);
            var result =
                this._deviceDataDao.GetDayStatisticAggregateData(device.HardwareDeviceId, fieldId, startTime, endTime);
            return result;
        }

        public Object GetMonthAggregateDataByDeviceNameAndField(String deviceName, String fieldId, DateTime startTime, DateTime endTime)
        {
            DeviceModel device = this._deviceDao.GetByDeviceNamePrecise(deviceName);
            var result =
                this._deviceDataDao.GetMonthAggregateData(device.HardwareDeviceId, fieldId, startTime, endTime);
            return result;
        }
    }
}