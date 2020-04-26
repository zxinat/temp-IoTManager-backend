using System;
using System.Collections.Generic;
using System.Linq;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace IoTManager.Dao
{
    public sealed class AlarmInfoDao: IAlarmInfoDao
    {
        private readonly IMongoCollection<AlarmInfoModel> _alarmInfo;

        public AlarmInfoDao()
        {
            var client = new MongoClient(Constant.getMongoDBConnectionString());
            var database = client.GetDatabase("iotmanager");

            _alarmInfo = database.GetCollection<AlarmInfoModel>("alarminfo");
        }

        public List<AlarmInfoModel> Get(String searchType, String deviceId = "all", int offset = 0, int limit = 12, String sortColumn = "Id", String order = "asc")
        {
            if (searchType == "all")
            {
                return this._alarmInfo.AsQueryable()
                    .Where(ai => true)
                    .ToList();
            }
            else
            {
                var query = deviceId != "all"
                    ? this._alarmInfo.AsQueryable().Where(ai => ai.DeviceId == deviceId)
                    : this._alarmInfo.AsQueryable().Where(dd => true);
                var beforeOrder = query.OrderByDescending(ai => ai.Timestamp)
                    .Take(60)
                    .ToList();
                var ordered = order == "asc"
                    ? beforeOrder.OrderBy(ai => ai.GetType().GetProperty(sortColumn).GetValue(ai))
                    : beforeOrder.OrderByDescending(ai => ai.GetType().GetProperty(sortColumn).GetValue(ai));
                return ordered.Skip(offset)
                    .Take(limit)
                    .ToList();
            }
        }

        public AlarmInfoModel GetById(String Id)
        {
            return _alarmInfo.Find<AlarmInfoModel>(a => a.Id == Id).FirstOrDefault();
        }
        /*zxin-修改：获取最新指定数量的告警信息*/
        public List<AlarmInfoModel> GetByDeviceId(String deviceId,int count)
        {
            var query = this._alarmInfo.AsQueryable()
                .Where(ai => ai.DeviceId == deviceId)
                .OrderByDescending(ai => ai.Timestamp)
                .Take(count)
                .ToList();
            return query;
        }
        public List<AlarmInfoModel> GetByDeviceId20(String DeviceId)
        {
            var query = this._alarmInfo.AsQueryable()
                .Where(ai => ai.DeviceId == DeviceId)
                .OrderByDescending(ai => ai.Timestamp)
                .Take(20)
                .ToList();
            return query;
        }

        public List<AlarmInfoModel> GetByIndexId(String IndexId)
        {
            return _alarmInfo.Find<AlarmInfoModel>(a => a.IndexId == IndexId).ToList();
        }

        public String Create(AlarmInfoModel alarmInfoModel)
        {
            _alarmInfo.InsertOne(alarmInfoModel);
            return "success";
        }

        public List<AlarmInfoModel> GetFiveInfo()
        {
            var query = this._alarmInfo.AsQueryable()
                .Where(ai => true)
                .OrderByDescending(ai => ai.Timestamp)
                .Take(5)
                .ToList();
            return query;
        }

        public int GetNoticeAlarmInfoAmount()
        {
            /*
            List<AlarmInfoModel> alarmInfos = _alarmInfo.Find<AlarmInfoModel>(a => a.Severity=="Info").ToList();
            return alarmInfos.Count;
            */
            FilterDefinition<AlarmInfoModel> filter = Builders<AlarmInfoModel>.Filter.Eq("Severity", "Info");
            return (int)_alarmInfo.Find(filter).CountDocuments();
        }

        public int GetSeriousAlarmInfoAmount()
        {
            /*
            List<AlarmInfoModel> alarmInfos = _alarmInfo.Find<AlarmInfoModel>(a => a.Severity=="Warning").ToList();
            return alarmInfos.Count;
            */
            FilterDefinition<AlarmInfoModel> filter = Builders<AlarmInfoModel>.Filter.Eq("Severity", "Warning");
            return (int)_alarmInfo.Find(filter).CountDocuments();
        }

        public int GetVerySeriousAlarmInfoAmount()
        {
            /*
            List<AlarmInfoModel> alarmInfos = _alarmInfo.Find<AlarmInfoModel>(a => a.Severity=="Critical").ToList();
            return alarmInfos.Count;
            */
            FilterDefinition<AlarmInfoModel> filter = Builders<AlarmInfoModel>.Filter.Eq("Severity", "Critical");
            return (int)_alarmInfo.Find(filter).CountDocuments();
        }

        public String UpdateProcessed(String id)
        {
            var filter = Builders<AlarmInfoModel>.Filter.Eq("_id", new ObjectId(id));
            var update = Builders<AlarmInfoModel>.Update.Set("Processed", "Yes");
            var result = this._alarmInfo.UpdateOne(filter, update);
            return result.ModifiedCount == 1 ? "success" : "error";
        }

        public long GetAlarmInfoNumber(String searchType, String deviceId = "all")
        {
            FilterDefinition<AlarmInfoModel> filter;
            if (searchType == "search")
            {
                if (deviceId != "all")
                {
                    /*
                    var query = this._alarmInfo.AsQueryable()
                        .Where(ai => ai.DeviceId == deviceId)
                        .Take(60)
                        .ToList();
                    */
                    /*zxin-修改查询方式*/
                    filter = Builders<AlarmInfoModel>.Filter.Eq("DeviceId", deviceId);
                    
                }
                else
                {
                    /*
                    var query = this._alarmInfo.AsQueryable()
                        .Where(ai => true)
                        .Take(60)
                        .ToList();
                    return query.Count;
                    */
                    filter = Builders<AlarmInfoModel>.Filter.Empty;
                }
            }
            else
            {
                /*
                var query = this._alarmInfo.AsQueryable()
                    .Where(ai => true)
                    .ToList();
                return query.Count;
                */
                filter = Builders<AlarmInfoModel>.Filter.Empty;
            }
            return this._alarmInfo.Find(filter).CountDocuments();
        }
        public String Delete(String id)
        {
            var filter = Builders<AlarmInfoModel>.Filter.Eq("_id", new ObjectId(id));
            var result = this._alarmInfo.DeleteOne(filter);
            return result.DeletedCount == 1 ? "success" : "error";
        }
        /* 输入：设备Id、起止时间
         * 输出：总告警次数
         */
        public int GetDeviceAffiliateAlarmInfoNumber(String deviceId,DateTime startTime,DateTime endTime)
        {
            FilterDefinitionBuilder<AlarmInfoModel> builderFilter = Builders<AlarmInfoModel>.Filter;
            FilterDefinition<AlarmInfoModel> filter = builderFilter.And(builderFilter.Eq("DeviceId", deviceId),
                builderFilter.Gt("Timestamp", startTime),
                builderFilter.Lt("Timestamp", endTime));
            return (int)_alarmInfo.Find(filter).CountDocuments();
            /*
            return _alarmInfo.AsQueryable()
                .Where(ai => ai.DeviceId == deviceId & ai.Timestamp>=startTime &ai.Timestamp<=endTime)
                .ToList().Count;
             */
        }
        /* 输入：起止时间
         * 输出：时间段内数据总数
         * 
         */
        public int GetDeviceAlarmInfoNumberByTime(DateTime startTime,DateTime endTime)
        {
            FilterDefinitionBuilder<AlarmInfoModel> builderFilter = Builders<AlarmInfoModel>.Filter;
            FilterDefinition<AlarmInfoModel> filter = builderFilter.And(
                builderFilter.Gt("Timestamp", startTime),
                builderFilter.Lt("Timestamp", endTime));
            return (int)_alarmInfo.Find(filter).CountDocuments();
        }

        public int BatchDelete(List<String> ids)
        {
            foreach (String id in ids)
            {
                var filter = Builders<AlarmInfoModel>.Filter.Eq("_id", new ObjectId(id));
                var result = this._alarmInfo.DeleteOne(filter);
            }

            return ids.Count;
        }
    }
}