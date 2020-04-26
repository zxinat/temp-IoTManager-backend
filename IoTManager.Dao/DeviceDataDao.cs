using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MySql.Data.MySqlClient;
using System.Linq.Dynamic;
using IoTManager.Model.DataReceiver;
using Newtonsoft.Json;

namespace IoTManager.Dao
{
    public sealed class DeviceDataDao: IDeviceDataDao
    {
        private readonly IMongoCollection<DeviceDataModel> _deviceData;
        private readonly IMongoCollection<AlarmInfoModel> _alarmInfo;
        private readonly IThresholdDao _thresholdDao;
        private readonly IFieldDao _fieldDao;
        private readonly ICityDao _cityDao;

        public DeviceDataDao(IThresholdDao thresholdDao, IFieldDao fieldDao, ICityDao cityDao)
        {
            var client = new MongoClient(Constant.getMongoDBConnectionString());
            var database = client.GetDatabase("iotmanager");
            _deviceData = database.GetCollection<DeviceDataModel>("monitordata");
            _alarmInfo = database.GetCollection<AlarmInfoModel>("alarminfo");
            this._thresholdDao = thresholdDao;
            this._fieldDao = fieldDao;
            this._cityDao = cityDao;
        }

        public List<DeviceDataModel> Get(String searchType, String deviceId = "all", int offset = 0, int limit = 12, String sortColumn = "Id", String order = "asc")
        {
            var filter = deviceId == "all"
                    ? Builders<DeviceDataModel>.Filter.Empty
                    : Builders<DeviceDataModel>.Filter.Eq("DeviceId", deviceId);
            SortDefinition<DeviceDataModel> sort = order == "asc"
                ? Builders<DeviceDataModel>.Sort.Ascending(sortColumn)
                : Builders<DeviceDataModel>.Sort.Descending(sortColumn);
            var query=searchType=="all"?
                this._deviceData.AsQueryable()
                    .Where(dd => true)
                    .ToList():
                    this._deviceData.Find(filter).Sort(sort).Limit(limit).Skip(offset).ToList();
            return query;
            /*
            if (searchType == "all")
            {
                return this._deviceData.AsQueryable()
                    .Where(dd => true)
                    .ToList();
            }
            else
            {   /*
                var query = deviceId != "all"
                    ? this._deviceData.AsQueryable()
                        .Where(dd => dd.DeviceId == deviceId)
                    : this._deviceData.AsQueryable()
                        .Where(dd => true);
                var beforeOrder = query.OrderByDescending(dd => dd.Timestamp).Take(60).ToList();
                var ordered = order == "asc"
                    ? beforeOrder.OrderBy(dd => dd.GetType().GetProperty(sortColumn).GetValue(dd))
                    : beforeOrder.OrderByDescending(dd => dd.GetType().GetProperty(sortColumn).GetValue(dd));
                List<DeviceDataModel> result = ordered.Skip(offset).Take(limit).ToList();
                return result;
                *//*
                var filter = deviceId == "all"
                    ? Builders<DeviceDataModel>.Filter.Empty
                    : Builders<DeviceDataModel>.Filter.Eq("DeviceId", deviceId);  
                SortDefinition<DeviceDataModel> sort = order == "asc"
                    ? Builders<DeviceDataModel>.Sort.Ascending(sortColumn)
                    : Builders<DeviceDataModel>.Sort.Descending(sortColumn);
                return this._deviceData.Find(filter).Sort(sort).Limit(limit).Skip(offset).ToList();
            }
            */
            
        }

        public DeviceDataModel GetById(String Id)
        {
            DeviceDataModel deviceData= _deviceData.Find<DeviceDataModel>(dd => dd.Id == Id).FirstOrDefault();
            deviceData.Timestamp += TimeSpan.FromHours(8);
            return deviceData;
        }

        public List<DeviceDataModel> GetByDeviceId(String deviceId)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .ToList();
            if (query.Count != 0)
            {
                foreach (var d in query)
                {
                    d.Timestamp += TimeSpan.FromHours(8);
                }
            }
            return query;
        }
        /*zxin-根据设备名称获取最新的count条数据*/
        public List<DeviceDataModel> GetByDeviceName(String deviceName,int count)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceName == deviceName)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(count)
                .ToList();
            if (query.Count != 0)
            {
                foreach (var d in query)
                {
                    d.Timestamp += TimeSpan.FromHours(8);
                }
            }
            return query;
        }
        /*zxin-根据设备名称获取最早的count条数据*/
        public List<DeviceDataModel> ListByDeviceNameASC(string deviceName,int count)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceName == deviceName)
                .OrderBy(dd => dd.Timestamp)
                .Take(count)
                .ToList();
            if (query.Count != 0)
            {
                foreach (var d in query)
                {
                    d.Timestamp += TimeSpan.FromHours(8);
                }
            }
            return query;
        }
        /*通过deviceId获取最新一条数据*/
        public DeviceDataModel GetLastDataByDeviceId(string deviceId)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(1).FirstOrDefault();
            query.Timestamp += TimeSpan.FromHours(8);
            return query;      
        }

        public List<DeviceDataModel> GetByDeviceId20(String DeviceId)
        {
            
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == DeviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(20)
                .ToList();
            if (query.Count != 0)
            {
                foreach (var d in query)
                {
                    d.Timestamp += TimeSpan.FromHours(8);
                }
            }

            //var filter = Builders<DeviceDataModel>.Filter.Eq("DeviceId", DeviceId);
            //var query = this._deviceData.Find(filter).Limit(20).Skip(0).ToList();
            return query; 
        }

        public List<DeviceDataModel> GetByDeviceId100(String deviceId)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(100)
                .ToList();
            return query;
        }

        public List<DeviceDataModel> GetAllDataByDeviceId(String DeviceId)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == DeviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .ToList();
            return query;
        }

        public List<DeviceDataModel> GetNotInspected()
        {
            List<DeviceDataModel> deviceDataModels = _deviceData.Find<DeviceDataModel>(dd => dd.Inspected == "false").ToList();
            var filter = Builders<DeviceDataModel>.Filter.Eq("IsScam", "false");
            var update = Builders<DeviceDataModel>.Update.Set("IsScam", "true");
            var result = _deviceData.UpdateMany(filter, update);
            return deviceDataModels;
        }

        public Object GetLineChartData(String deviceId, String indexId)
        {
            List<double> chartValue = new List<double>();
            List<String> xAxises = new List<string>();

            var query = this._deviceData.AsQueryable()
                .Where(dd => (dd.DeviceId == deviceId && dd.IndexId == indexId))
                .OrderByDescending(dd => dd.Timestamp)
                .Take(12)
                .ToList();

            foreach (var dd in query)
            {
                chartValue.Add(dd.IndexValue);
                xAxises.Add(DateTime.Parse(dd.Timestamp.ToString()).ToLocalTime()
                    .ToString(Constant.getDateFormatString()));
            }

            xAxises.Reverse();
            chartValue.Reverse();

            return new {xAxis = xAxises, series = chartValue, indexId = indexId};
        }

        public int GetDeviceDataAmount()
        {
            List<DeviceDataModel> deviceData = _deviceData.Find<DeviceDataModel>(dd => true).ToList();
            return deviceData.Count;
        }
        /*没用上*/
        public object GetDeviceStatusById(int id, DateTime sTime, DateTime eTime)
        {
            //TODO: Optimize this function
            String deviceId = "";
            DeviceModel device = new DeviceModel();
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                device = connection.Query<DeviceModel>("select device.id, " +
                                                       "hardwareDeviceID, " +
                                                       "deviceName, " +
                                                       "city.cityName as city, " +
                                                       "factory.factoryName as factory, " +
                                                       "workshop.workshopName as workshop, " +
                                                       "deviceState, " +
                                                       "imageUrl, " +
                                                       "gatewayID, " +
                                                       "mac, " +
                                                       "deviceType, " +
                                                       "device.remark, " +
                                                       "lastConnectionTime, " +
                                                       "device.createTime, " +
                                                       "device.updateTime, " +
                                                       "device.pictureRoute, " +
                                                       "isOnline, " + 
                                                       "base64Image " +
                                                       "from device " +
                                                       "join city on city.id=device.city " +
                                                       "join factory on factory.id=device.factory " +
                                                       "join workshop on workshop.id=device.workshop " + 
                                                       "where device.id=@did", new {did = id})
                    .FirstOrDefault();
                deviceId = device.HardwareDeviceId;
            }

            var lastQuery = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(1)
                .ToList();
            
            var firstQuery = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .OrderBy(dd => dd.Timestamp)
                .Take(1)
                .ToList();

            var alarmTimeQuery = this._alarmInfo.AsQueryable()
                .Where(ai => ai.DeviceId == deviceId)
                .ToList();

            var recentAlarmQuery = this._alarmInfo.AsQueryable()
                .Where(ai => ai.DeviceId == deviceId)
                .OrderByDescending(ai => ai.Timestamp)
                .Take(1)
                .ToList();

            var alarmQuery = this._alarmInfo.AsQueryable()
                .Where(ai => ai.DeviceId == deviceId)
                .OrderByDescending(ai => ai.Timestamp)
                .Take(10)
                .ToList();

            var deviceDataQuery = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(10)
                .ToList();

            foreach (var a in alarmQuery)
            {
                a.DeviceId = a.Timestamp.ToString(Constant.getDateFormatString());
            }

            foreach (var d in deviceDataQuery)
            {
                d.DeviceId = d.Timestamp.ToString(Constant.getDateFormatString());
            }

            DeviceDataModel lastDeviceData = new DeviceDataModel();
            DeviceDataModel firstDeviceData = new DeviceDataModel();
            DateTime startTime = DateTime.MinValue;
            TimeSpan lastingTime = TimeSpan.Zero;
            ;
            if (lastQuery.Count > 0 && firstQuery.Count > 0)
            {
                lastDeviceData = lastQuery[0];
                firstDeviceData = firstQuery[0];
                startTime = firstDeviceData.Timestamp;
                lastingTime = lastDeviceData.Timestamp - firstDeviceData.Timestamp;
            }
            int alarmTimes = alarmTimeQuery.Count;
            DateTime recentAlarm = DateTime.MinValue;
            if (recentAlarmQuery.Count > 0)
            {
                recentAlarm = recentAlarmQuery[0].Timestamp;
            }

            List<ThresholdModel> thresholdModels = this._thresholdDao.GetByDeviceId(deviceId);
            List<object> ruleResult = new List<object>();
            Dictionary<String, String> opDict = new Dictionary<string, string>();
            opDict.Add("greater", ">");
            opDict.Add("equal", "=");
            opDict.Add("less", "<");
            foreach (ThresholdModel t in thresholdModels)
            {
                ruleResult.Add(new {name=t.RuleName, description=t.Description, conditionString=t.IndexId + opDict[t.Operator.ToString()] + t.ThresholdValue.ToString(), severity=t.Severity});
            }

            List<FieldModel> fields = this._fieldDao.Get();
            var affiliateFields = fields.AsQueryable()
                .Where(f => f.Device == device.DeviceName)
                .ToList();
            List<object> aggregateDayResult = new List<object>();
            List<object> aggregateHourResult = new List<object>();
            List<object> aggregateMonthResult = new List<object>();
            List<String> nonNullFields = new List<string>();
            foreach (var f in affiliateFields)
            {
                var tmpDay = GetDayStatisticAggregateData(device.HardwareDeviceId, f.FieldId, sTime, eTime);
                aggregateDayResult.Add(tmpDay);

                var tmpHour = GetHourAggregateData(device.HardwareDeviceId, f.FieldId, sTime, eTime);
                aggregateHourResult.Add(tmpHour);

                var tmpMonth = GetMonthAggregateData(device.HardwareDeviceId, f.FieldId, sTime, eTime);
                aggregateMonthResult.Add(tmpMonth);
            }

            CityModel city = this._cityDao.GetOneCityByName(device.City);

            Dictionary<String, object> resultHundredData = new Dictionary<string, object>();
            foreach (var f in affiliateFields)
            {
                var hundredData = this._deviceData.AsQueryable()
                    .Where(dd => dd.DeviceId == device.HardwareDeviceId && dd.IndexId == f.FieldId)
                    .OrderByDescending(dd => dd.Timestamp)
                    .Take(100)
                    .ToList();
                foreach (var d in hundredData)
                {
                    d.Timestamp = DateTime.Parse(d.Timestamp.ToString())
                        .ToLocalTime();
                }
                hundredData.Reverse();
                resultHundredData.Add(f.FieldId, hundredData);
            }

            return new
            {
                id = device.Id,
                deviceName = device.DeviceName,
                hardwareDeviceID = device.HardwareDeviceId,
                base64Image = device.Base64Image,
                deviceType = device.DeviceType,
                deviceState = device.DeviceState,
                imageUrl = device.ImageUrl,
                alarmInfo = alarmQuery,
                deviceData = deviceDataQuery,
                startTime = startTime == DateTime.MinValue ? "未收到数据" : startTime.ToString(Constant.getDateFormatString()),
                runningTime = lastingTime == TimeSpan.Zero ? "未收到数据" : lastingTime.ToString("%d") + "天" + 
                              lastingTime.ToString("%h") + "小时" + 
                              lastingTime.ToString("%m") + "分钟" + 
                              lastingTime.ToString("%s") + "秒",
                alarmTimes = alarmTimes.ToString(),
                recentAlarmTime = recentAlarm == DateTime.MinValue ? "未收到数据" : recentAlarm.ToString(Constant.getDateFormatString()),
                rules = ruleResult,
                fields = affiliateFields,
                aggregateDayResult = aggregateDayResult,
                aggregateHourResult = aggregateHourResult,
                aggregateMonthResult = aggregateMonthResult,
                city = device.City,
                longitude = city.longitude,
                latitude = city.latitude,
                affiliateFields = affiliateFields,
                hundredData = resultHundredData
            };
        }

        public object GetDeviceStatistic(StatisticDurationModel statisticDurationModel)
        {
            return null;
        }

        public long GetDeviceDataNumber(String searchType, String deviceId = "all")
        {
            FilterDefinition<DeviceDataModel> filter;
            
            if (searchType == "search")
            {
                if (deviceId != "all")
                {
                    /*
                    var query = this._deviceData.AsQueryable()
                        .Where(dd => dd.DeviceId == deviceId)
                        .Take(60)
                        .ToList();
                    return query.Count;
                    */
                    filter = Builders<DeviceDataModel>.Filter.Eq("DeviceId", deviceId);
                }
                else
                {
                    /*
                    var query = this._deviceData.AsQueryable()
                        .Where(dd => true)
                        .Take(60)
                        .ToList();
                    return query.Count;
                    */
                    filter = Builders<DeviceDataModel>.Filter.Empty;
                }
            }
            else
            {
                /*
                var query = this._deviceData.AsQueryable()
                    .Where(dd => true)
                    .ToList();
                return query.Count;
                */
                filter = Builders<DeviceDataModel>.Filter.Empty;
            }
            return this._deviceData.Find(filter).CountDocuments();
        }

        public String Delete(String id)
        {
            var filter = Builders<DeviceDataModel>.Filter.Eq("_id", new ObjectId(id));
            var result = this._deviceData.DeleteOne(filter);
            return result.DeletedCount == 1 ? "success" : "error";
        }

        public int BatchDelete(List<String> ids)
        {
            int num = 0;
            foreach (String id in ids)
            {
                var filter = Builders<DeviceDataModel>.Filter.Eq("_id", new ObjectId(id));
                var result = this._deviceData.DeleteOne(filter);
                num = num + 1;
            }
            return num;
        }

        public String Update(String id, DeviceDataModel deviceDataModel)
        {
            var filter = Builders<DeviceDataModel>.Filter.Eq("_id", new ObjectId(id));
            var update = Builders<DeviceDataModel>.Update.Set("IndexValue", deviceDataModel.IndexValue.ToString());
            var result = _deviceData.UpdateOne(filter, update);
            return result.ModifiedCount == 1 ? "success" : "error";
        }

        public List<StatisticDataModel> GetDayAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime, String scale)
        {
            if (scale == "day")
            {
                var result = GetDayStatisticAggregateData(deviceId, indexId, startTime, endTime);
                /*
                var avg = this._deviceData.Aggregate()
                    .Match(dd =>
                        dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime &&
                        dd.Timestamp <= endTime)
                    .Project(dd => new
                    {
                        year = dd.Timestamp.Year,
                        month = dd.Timestamp.Month,
                        day = dd.Timestamp.Day,
                        DeviceId = dd.DeviceId,
                        IndexId = dd.IndexId,
                        IndexValue = dd.IndexValue
                    })
                    .Group(x => new {year = x.year, month = x.month, day = x.day},
                        g => new {time = g.Key, avg = g.Average(x => x.IndexValue)})
                    .ToList();
                var max = this._deviceData.Aggregate()
                    .Match(dd =>
                        dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime &&
                        dd.Timestamp <= endTime)
                    .Project(dd => new
                    {
                        year = dd.Timestamp.Year,
                        month = dd.Timestamp.Month,
                        day = dd.Timestamp.Day,
                        DeviceId = dd.DeviceId,
                        IndexId = dd.IndexId,
                        IndexValue = dd.IndexValue
                    })
                    .Group(x => new {year = x.year, month = x.month, day = x.day},
                        g => new {time = g.Key, max = g.Max(x => x.IndexValue)})
                    .ToList();
                var min = this._deviceData.Aggregate()
                    .Match(dd =>
                        dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime &&
                        dd.Timestamp <= endTime)
                    .Project(dd => new
                    {
                        year = dd.Timestamp.Year,
                        month = dd.Timestamp.Month,
                        day = dd.Timestamp.Day,
                        DeviceId = dd.DeviceId,
                        IndexId = dd.IndexId,
                        IndexValue = dd.IndexValue
                    })
                    .Group(x => new {year = x.year, month = x.month, day = x.day},
                        g => new {time = g.Key, min = g.Min(x => x.IndexValue)})
                    .ToList();
                List<object> result = new List<object>();
                for (int i = 0; i < avg.Count; i++)
                {
                    String t = avg[i].time.year + "-" +
                               avg[i].time.month + "-" +
                               avg[i].time.day;
                    result.Add(new
                    {
                        time = t,
                        index = indexId,
                        max = max[i].max,
                        min = min[i].min,
                        avg = avg[i].avg
                    });
                }*/

                return result;
            } 
            else if (scale == "month")
            {
                /*
                var avg = this._deviceData.Aggregate()
                    .Match(dd =>
                        dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime &&
                        dd.Timestamp <= endTime)
                    .Project(dd => new
                    {
                        year = dd.Timestamp.Year,
                        month = dd.Timestamp.Month,
                        DeviceId = dd.DeviceId,
                        IndexId = dd.IndexId,
                        IndexValue = dd.IndexValue
                    })
                    .Group(x => new {year = x.year, month = x.month},
                        g => new {time = g.Key, avg = g.Average(x => x.IndexValue)})
                    .ToList();
                var max = this._deviceData.Aggregate()
                    .Match(dd =>
                        dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime &&
                        dd.Timestamp <= endTime)
                    .Project(dd => new
                    {
                        year = dd.Timestamp.Year,
                        month = dd.Timestamp.Month,
                        DeviceId = dd.DeviceId,
                        IndexId = dd.IndexId,
                        IndexValue = dd.IndexValue
                    })
                    .Group(x => new {year = x.year, month = x.month},
                        g => new {time = g.Key, max = g.Max(x => x.IndexValue)})
                    .ToList();
                var min = this._deviceData.Aggregate()
                    .Match(dd =>
                        dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime &&
                        dd.Timestamp <= endTime)
                    .Project(dd => new
                    {
                        year = dd.Timestamp.Year,
                        month = dd.Timestamp.Month,
                        DeviceId = dd.DeviceId,
                        IndexId = dd.IndexId,
                        IndexValue = dd.IndexValue
                    })
                    .Group(x => new {year = x.year, month = x.month},
                        g => new {time = g.Key, min = g.Min(x => x.IndexValue)})
                    .ToList();
                List<object> result = new List<object>();
                for (int i = 0; i < avg.Count; i++)
                {
                    String t = avg[i].time.year + "-" +
                               avg[i].time.month;
                    result.Add(new
                    {
                        time = t,
                        index = indexId,
                        max = max[i].max,
                        min = min[i].min,
                        avg = avg[i].avg
                    });
                }
                */
                var result = GetMonthAggregateData(deviceId, indexId, startTime, endTime);
                return result;
            }
            else if (scale == "hour")
            {
                /*
                var avg = this._deviceData.Aggregate()
                    .Match(dd =>
                        dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime &&
                        dd.Timestamp <= endTime)
                    .Project(dd => new
                    {
                        year = dd.Timestamp.Year,
                        month = dd.Timestamp.Month,
                        day = dd.Timestamp.Day,
                        hour = dd.Timestamp.Hour,
                        DeviceId = dd.DeviceId,
                        IndexId = dd.IndexId,
                        IndexValue = dd.IndexValue
                    })
                    .Group(x => new {year = x.year, month = x.month, day = x.day, hour = x.hour},
                        g => new {time = g.Key, avg = g.Average(x => x.IndexValue)})
                    .ToList();
                var max = this._deviceData.Aggregate()
                    .Match(dd =>
                        dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime &&
                        dd.Timestamp <= endTime)
                    .Project(dd => new
                    {
                        year = dd.Timestamp.Year,
                        month = dd.Timestamp.Month,
                        day = dd.Timestamp.Day,
                        hour = dd.Timestamp.Hour,
                        DeviceId = dd.DeviceId,
                        IndexId = dd.IndexId,
                        IndexValue = dd.IndexValue
                    })
                    .Group(x => new {year = x.year, month = x.month, day = x.day, hour = x.hour},
                        g => new {time = g.Key, max = g.Max(x => x.IndexValue)})
                    .ToList();
                var min = this._deviceData.Aggregate()
                    .Match(dd =>
                        dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime &&
                        dd.Timestamp <= endTime)
                    .Project(dd => new
                    {
                        year = dd.Timestamp.Year,
                        month = dd.Timestamp.Month,
                        day = dd.Timestamp.Day,
                        hour = dd.Timestamp.Hour,
                        DeviceId = dd.DeviceId,
                        IndexId = dd.IndexId,
                        IndexValue = dd.IndexValue
                    })
                    .Group(x => new {year = x.year, month = x.month, day = x.day, hour = x.hour},
                        g => new {time = g.Key, min = g.Min(x => x.IndexValue)})
                    .ToList();
                List<object> result = new List<object>();
                for (int i = 0; i < avg.Count; i++)
                {
                    var tmpt = avg[i].time.hour + 8;
                    var tmpd = avg[i].time.day;
                    if (tmpt > 24)
                    {
                        tmpt = tmpt % 24;
                        tmpd = tmpd + 1;
                    }
                    String t = avg[i].time.year + "-" +
                               avg[i].time.month + "-" +
                               tmpd.ToString() + " " +
                               tmpt.ToString() ;
                    result.Add(new
                    {
                        time = t,
                        index = indexId,
                        max = max[i].max,
                        min = min[i].min,
                        avg = avg[i].avg
                    });
                }
                */
                /*zxin优化时数据查询*/
                var result = GetHourAggregateData(deviceId, indexId, startTime, endTime);
                return result;

            }
            else
            {
                return null;
            }
        }
        
        public List<StatisticDataModel> GetDayStatisticAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime)
        {
            endTime += TimeSpan.FromDays(1);
            var data = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp  <= endTime)
                .Project(dd => new
                {
                    date=dd.Timestamp,
                    year = dd.Timestamp.Year,
                    month = dd.Timestamp.Month,
                    day = dd.Timestamp .Day,
                    hour=dd.Timestamp.Hour,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new {year = x.year, month = x.month, day = x.day,days=(x.year*366+x.month*31+x.day)},
                    g => new {time = g.Key, avg = g.Average(x => x.IndexValue),min=g.Min(x=>x.IndexValue),max=g.Max(x=>x.IndexValue)}).SortBy(s=>s.time.days)
                .ToList();
            var result = new List<StatisticDataModel>();
            foreach (var d in data)
            {

                result.Add(new StatisticDataModel
                {
                    time = String.Format("{0}-{1}-{2}", d.time.year, d.time.month, d.time.day),
                    index = indexId,
                    max = (float)d.max,
                    min = (float)d.min,
                    avg = (float)d.avg,
                    date = new DateTime(d.time.year, d.time.month, d.time.day)
                    
                });
            }
            /*
            var max = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                .Project(dd => new
                {
                    year = dd.Timestamp.Year,
                    month = dd.Timestamp.Month,
                    day = dd.Timestamp.Day,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new {year = x.year, month = x.month, day = x.day},
                    g => new {time = g.Key, max = g.Max(x => x.IndexValue)})
                .ToList();
            var min = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                .Project(dd => new
                {
                    year = dd.Timestamp.Year,
                    month = dd.Timestamp.Month,
                    day = dd.Timestamp.Day,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new {year = x.year, month = x.month, day = x.day},
                    g => new {time = g.Key, min = g.Min(x => x.IndexValue)})
                .ToList();
            */
            return result;
            ;
        }
        
        public List<StatisticDataModel> GetHourAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime)
        {
            /*
            var avg = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                .Project(dd => new
                {
                    year = dd.Timestamp.Year,
                    month = dd.Timestamp.Month,
                    day = dd.Timestamp.Day,
                    hour = dd.Timestamp.Hour,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new {year = x.year, month = x.month, day = x.day, hour = x.hour},
                    g => new {time = g.Key, avg = g.Average(x => x.IndexValue)})
                .ToList();
            var max = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                .Project(dd => new
                {
                    year = dd.Timestamp.Year,
                    month = dd.Timestamp.Month,
                    day = dd.Timestamp.Day,
                    hour = dd.Timestamp.Hour,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new {year = x.year, month = x.month, day = x.day, hour = x.hour},
                    g => new {time = g.Key, max = g.Max(x => x.IndexValue)})
                .ToList();
            var min = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                .Project(dd => new
                {
                    year = dd.Timestamp.Year,
                    month = dd.Timestamp.Month,
                    day = dd.Timestamp.Day,
                    hour = dd.Timestamp.Hour,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new {year = x.year, month = x.month, day = x.day, hour = x.hour},
                    g => new {time = g.Key, min = g.Min(x => x.IndexValue)})
                .ToList();
            return new {index = indexId, min = min, avg = avg, max = max};
            */
            /*zxin-优化查询方式*/
            endTime += TimeSpan.FromDays(1);
            var data = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp  <= endTime)
                .Project(dd => new
                {
                    date=dd.Timestamp,
                    year = dd.Timestamp .Year,
                    month = dd.Timestamp .Month,
                    day = dd.Timestamp .Day,
                    hour= dd.Timestamp.Hour,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new { year = x.year, month = x.month, day = x.day, hour=x.hour,hours = (x.year * 366 + x.month * 31 + x.day)*24+x.hour},
                    g => new { time = g.Key, avg = g.Average(x => x.IndexValue), min = g.Min(x => x.IndexValue), max = g.Max(x => x.IndexValue) }).SortBy(s => s.time.hours)
                .ToList();
            List<StatisticDataModel> result = new List<StatisticDataModel>();
            foreach(var d in data)
            {
                DateTime date = new DateTime(d.time.year, d.time.month, d.time.day, d.time.hour, 0, 0) + TimeSpan.FromHours(8);
                result.Add(new StatisticDataModel
                {
                    time = String.Format("{0}-{1}-{2} {3}", date.Year, date.Month, date.Day,date.Hour),
                    index = indexId,
                    max = (float)d.max,
                    min = (float)d.min,
                    avg = (float)d.avg,
                    date=date
                    
                });
            }
            return result;
        }
        
        public List<StatisticDataModel> GetMonthAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime)
        {
            /*
            var avg = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                .Project(dd => new
                {
                    year = dd.Timestamp.Year,
                    month = dd.Timestamp.Month,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new {year = x.year, month = x.month},
                    g => new {time = g.Key, avg = g.Average(x => x.IndexValue)})
                .ToList();
            var max = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                .Project(dd => new
                {
                    year = dd.Timestamp.Year,
                    month = dd.Timestamp.Month,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new {year = x.year, month = x.month},
                    g => new {time = g.Key, max = g.Max(x => x.IndexValue)})
                .ToList();
            var min = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp >= startTime && dd.Timestamp <= endTime)
                .Project(dd => new
                {
                    year = dd.Timestamp.Year,
                    month = dd.Timestamp.Month,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new {year = x.year, month = x.month},
                    g => new {time = g.Key, min = g.Min(x => x.IndexValue)})
                .ToList();
            
            return new {index = indexId, avg = avg, min = min, max = max};
            */
            /*zxin--*/
            endTime += TimeSpan.FromDays(1);
            var data = this._deviceData.Aggregate()
                .Match(dd => dd.DeviceId == deviceId && dd.IndexId == indexId && dd.Timestamp  >= startTime && dd.Timestamp <= endTime)
                .Project(dd => new
                {
                    year =dd.Timestamp .Year,
                    month = dd.Timestamp .Month,
                    DeviceId = dd.DeviceId,
                    IndexId = dd.IndexId,
                    IndexValue = dd.IndexValue
                })
                .Group(x => new { year = x.year, month = x.month, months = x.year * 12 + x.month  },
                    g => new { time = g.Key, avg = g.Average(x => x.IndexValue), min = g.Min(x => x.IndexValue), max = g.Max(x => x.IndexValue) }).SortBy(s => s.time.months)
                .ToList();
            List<StatisticDataModel> result = new List<StatisticDataModel>();
            foreach (var d in data)
            {
                result.Add(new StatisticDataModel
                {
                    time = String.Format("{0}-{1}", d.time.year, d.time.month),
                    index = indexId,
                    max = (float)d.max,
                    min = (float)d.min,
                    avg = (float)d.avg,
                    date=new DateTime(d.time.year,d.time.month,1),
                    
                });
            }
            return result;
        }

        public int GetDeviceAffiliateData(String deviceId)
        {
            return this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .ToList().Count;
        }

        public int GetFieldAffiliateData(String fieldId)
        {
            return this._deviceData.AsQueryable()
                .Where(dd => dd.IndexId == fieldId)
                .ToList().Count;
        }
        /*获取date这一天00:00:00-23:59:59的所有设备数据*/
        public List<DeviceDataModel> GetByDate(DateTime date)
        {
            DateTime startTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.Timestamp >= startTime)
                .ToList();
            if (query.Count != 0)
            {
                foreach (var d in query)
                {
                    d.Timestamp += TimeSpan.FromHours(8);
                }
            }
            return query;
        }
        //根据deviceId 、MonitorId获取最近second秒的数据
        public List<DeviceDataModel> ListNewData(string deviceId,int second, string monitorId = "all")
        {
            DateTime now = DateTime.UtcNow;
            DateTime startTime= now-TimeSpan.FromSeconds(second);
            //DateTime startTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second - second);
            var query=monitorId=="all"? 
                this._deviceData.AsQueryable()
                    .Where(dd => dd.DeviceId == deviceId & dd.Timestamp >= startTime)
                    .ToList(): 
                this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId & dd.IndexId == monitorId & dd.Timestamp >= startTime)
                .ToList();
            //Utc时间转北京时间
            if(query.Count!=0)
            {
                foreach(var d in query)
                {
                    d.Timestamp += TimeSpan.FromHours(8);
                }
            }
            return query;
        }
        /*zxin-新增：获取不同安全级别下设备数*/
        public object GetDashboardDeviceStatus()
        {
            BsonDocument bd = new BsonDocument {
                { "_id",new BsonDocument{
                    { "severity", "$Severity" },
                    { "deviceId", "$DeviceId" } }
                }
            };
            BsonDocument bd1 = new BsonDocument { { "_id","$_id.severity" }, { "count", new BsonDocument("$sum", 1) } };
            var data = _alarmInfo.Aggregate()
                .Group(bd)
                .Group(bd1)
                .ToList();
            List<DashboardDeviceStatusModel> result = JsonConvert.DeserializeObject<List<DashboardDeviceStatusModel>>(data.ToJson());
            var infoResult = result.AsQueryable().Where(r => r._id == "Info").FirstOrDefault();
            var warningResult = result.AsQueryable().Where(r => r._id == "Warning").FirstOrDefault();
            var criticalResult = result.AsQueryable().Where(r => r._id == "Critical").FirstOrDefault();
            object res= new
            {
                info = infoResult != null ? infoResult.count : 0,
                warning = warningResult != null ? warningResult.count : 0,
                critical = criticalResult!=null? criticalResult.count:0
            };
            return res;
        }
    }
}