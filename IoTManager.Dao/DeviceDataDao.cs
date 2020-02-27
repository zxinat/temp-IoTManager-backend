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
            if (searchType == "all")
            {
                return this._deviceData.AsQueryable()
                    .Where(dd => true)
                    .ToList();
            }
            else
            {
                var query = deviceId != "all"
                    ? this._deviceData.AsQueryable()
                        .Where(dd => dd.DeviceId == deviceId)
                    : this._deviceData.AsQueryable()
                        .Where(dd => true);
                var beforeOrder = query.OrderByDescending(dd => dd.Timestamp)
                    .Take(60)
                    .ToList();
                var ordered = order == "asc"
                    ? beforeOrder.OrderBy(dd => dd.GetType().GetProperty(sortColumn).GetValue(dd))
                    : beforeOrder.OrderByDescending(dd => dd.GetType().GetProperty(sortColumn).GetValue(dd));
                return ordered.Skip(offset)
                    .Take(limit)
                    .ToList();
            }
        }

        public DeviceDataModel GetById(String Id)
        {
            return _deviceData.Find<DeviceDataModel>(dd => dd.Id == Id).FirstOrDefault();
        }

        public List<DeviceDataModel> GetByDeviceId(String deviceId)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .ToList();
            return query;
        }

        public List<DeviceDataModel> GetByDeviceId20(String DeviceId)
        {
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.DeviceId == DeviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(20)
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
            if (searchType == "search")
            {
                if (deviceId != "all")
                {
                    var query = this._deviceData.AsQueryable()
                        .Where(dd => dd.DeviceId == deviceId)
                        .Take(60)
                        .ToList();
                    return query.Count;
                }
                else
                {
                    var query = this._deviceData.AsQueryable()
                        .Where(dd => true)
                        .Take(60)
                        .ToList();
                    return query.Count;
                }
            }
            else
            {
                var query = this._deviceData.AsQueryable()
                    .Where(dd => true)
                    .ToList();
                return query.Count;
            }
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

        public object GetDayAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime, String scale)
        {
            if (scale == "day")
            {
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
                }

                return result;
            } 
            else if (scale == "month")
            {
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

                return result;
            }
            else if (scale == "hour")
            {
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

                return result;
            }
            else
            {
                return null;
            }
        }
        
        public object GetDayStatisticAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime)
        {
            var avg = this._deviceData.Aggregate()
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
                    g => new {time = g.Key, avg = g.Average(x => x.IndexValue)})
                .ToList();
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

            return new {index = indexId, avg = avg};
            ;
        }
        
        public object GetHourAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime)
        {
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
            return new {index = indexId, avg = avg};
        }
        
        public object GetMonthAggregateData(String deviceId, String indexId, DateTime startTime, DateTime endTime)
        {
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

        public List<DeviceDataModel> GetByDate(DateTime date)
        {
            DateTime startTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var query = this._deviceData.AsQueryable()
                .Where(dd => dd.Timestamp >= startTime)
                .ToList();
            return query;
        }
    }
}