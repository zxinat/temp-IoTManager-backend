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

        public List<AlarmInfoModel> GetByDeviceId(String DeviceId)
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
            List<AlarmInfoModel> alarmInfos = _alarmInfo.Find<AlarmInfoModel>(a => a.Severity=="Info").ToList();
            return alarmInfos.Count;
        }

        public int GetSeriousAlarmInfoAmount()
        {
            List<AlarmInfoModel> alarmInfos = _alarmInfo.Find<AlarmInfoModel>(a => a.Severity=="Warning").ToList();
            return alarmInfos.Count;
        }

        public int GetVerySeriousAlarmInfoAmount()
        {
            List<AlarmInfoModel> alarmInfos = _alarmInfo.Find<AlarmInfoModel>(a => a.Severity=="Critical").ToList();
            return alarmInfos.Count;
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
            if (searchType == "search")
            {
                if (deviceId != "all")
                {
                    var query = this._alarmInfo.AsQueryable()
                        .Where(ai => ai.DeviceId == deviceId)
                        .Take(60)
                        .ToList();
                    return query.Count;
                }
                else
                {
                    var query = this._alarmInfo.AsQueryable()
                        .Where(ai => true)
                        .Take(60)
                        .ToList();
                    return query.Count;
                }
            }
            else
            {
                var query = this._alarmInfo.AsQueryable()
                    .Where(ai => true)
                    .ToList();
                return query.Count;
            }
        }

        public String Delete(String id)
        {
            var filter = Builders<AlarmInfoModel>.Filter.Eq("_id", new ObjectId(id));
            var result = this._alarmInfo.DeleteOne(filter);
            return result.DeletedCount == 1 ? "success" : "error";
        }
    }
}