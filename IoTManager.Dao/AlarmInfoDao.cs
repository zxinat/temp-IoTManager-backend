using System;
using System.Collections.Generic;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
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

        public List<AlarmInfoModel> Get()
        {
            var query = this._alarmInfo.AsQueryable()
                .Where(ai => true)
                .OrderByDescending(ai => ai.Timestamp)
                .Take(20)
                .ToList();
            return query;
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
    }
}