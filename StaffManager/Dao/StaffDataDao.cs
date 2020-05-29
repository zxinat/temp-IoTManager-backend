using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StaffManager.Dao.Infrastructures;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using IoTManager.Utility;
using StaffManager.Models;
using IoTManager.Model;
using IoTManager.IDao;
using StaffManager.Models.StatisticModels;
using Microsoft.Extensions.Options;

namespace StaffManager.Dao
{
    public class StaffDataDao : IStaffDataDao
    {
        private readonly DatabaseConStr _databaseConStr;
        private readonly IMongoCollection<StaffDataModel> _staffData;
        private readonly IMongoCollection<AlarmInfoModel> _alarmInfo;
        private readonly IThresholdDao _thresholdDao;
        public StaffDataDao(IThresholdDao thresholdDao, IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
            var client = new MongoClient(_databaseConStr.MongoDB);
            var database = client.GetDatabase("iotmanager");
            _staffData = database.GetCollection<StaffDataModel>("monitordata");
            //_staffData = database.GetCollection<StaffDataModel>("");
            _alarmInfo = database.GetCollection<AlarmInfoModel>("alarminfo");
            this._thresholdDao = thresholdDao;
        }
        /* 根据deviceId、indexId获取一段时间内的count条最新数据
         */
        public List<StaffDataModel> ListByDeviceIdAndIndexId(string deviceId, string indexId, DateTime startTime, DateTime endTime, int count = 0)
        {
            var query = count != 0 ? _staffData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId & dd.IndexId == indexId & dd.Timestamp > startTime & dd.Timestamp < endTime)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(count)
                .ToList() : _staffData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId & dd.IndexId == indexId & dd.Timestamp > startTime & dd.Timestamp < endTime)
                .OrderByDescending(dd => dd.Timestamp)
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
        /*获取deviceId一段时间内的所有数据*/
        public List<StaffDataModel> ListByDeviceId(string deviceId, DateTime startTime, DateTime endTime, int count = 0)
        {
            var query = count == 0 ? _staffData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId & dd.Timestamp > startTime & dd.Timestamp < endTime)
                .OrderByDescending(dd => dd.Timestamp)
                .ToList()
                : _staffData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId & dd.Timestamp > startTime & dd.Timestamp < endTime)
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
        /*获取indexId一段时间最新的count条数据，count=0时获取所有*/
        public List<StaffDataModel> ListByIndexId(string indexId ,DateTime startTime, DateTime endTime, int count = 0)
        {
            var query = count == 0 ? _staffData.AsQueryable()
                .Where(dd => dd.IndexId == indexId & dd.Timestamp > startTime & dd.Timestamp < endTime)
                .OrderByDescending(dd => dd.Timestamp)
                .ToList()
                : _staffData.AsQueryable()
                .Where(dd => dd.IndexId == indexId & dd.Timestamp > startTime & dd.Timestamp < endTime)
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
        /*获取deviceId的value=indexValue最新count条数据*/
        public List<StaffDataModel> ListByDeviceIdAndValue(string deviceId, int indexValue, int count)
        {
            var query = _staffData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId & dd.IndexValue == indexValue)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(count).ToList();
            if (query.Count != 0)
            {
                foreach (var d in query)
                {
                    d.Timestamp += TimeSpan.FromHours(8);
                }
            }
            return query;
        }
        /*获取离岗数据*/
        public List<ExitInfoModel> ListExitInfo(string deviceId)
        {
            DateTime startTime = DateTime.Now.Date.ToUniversalTime();
            List<ExitInfoModel> exitInfos = new List<ExitInfoModel>();
            var query = _staffData.Aggregate()
                .SortByDescending(dd => dd.Timestamp)
                .Match(dd => dd.DeviceId == deviceId && dd.Timestamp > startTime && dd.Timestamp < startTime.AddDays(1))
                .Group(dd => new { tagId = dd.IndexId }, g => new { key = g.Key, timestamp = g.First().Timestamp, value = g.First().IndexValue })
                .Match(x => x.value == 0).ToList();
            foreach( var d in query)
            {
                exitInfos.Add(new ExitInfoModel
                {
                    tagId = d.key.tagId,
                    timestamp = d.timestamp
                });
            }
            return exitInfos;
        }
        /*通过indexId获取最新的一条数据*/
        public StaffDataModel GetByIndexIdLatestOne(string indexId)
        {
            var query = _staffData.AsQueryable()
                .Where(dd => dd.IndexId == indexId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(1).ToList().FirstOrDefault();
            return query;
        }
        public StaffDataModel GetLastDataByDeviceId(string deviceId)
        {
            var query = this._staffData.AsQueryable()
                .Where(dd => dd.DeviceId == deviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(1).FirstOrDefault();
            query.Timestamp += TimeSpan.FromHours(8);
            return query;
        }
        /* 根据deviceId获取一段时间内的所有数据
         */



        /*获取一段时间内打卡的所有标签列表*/
        public List<string> GetTagIds(DateTime startTime, DateTime endTime)
        {
            List<string> result = new List<string>();
            var data = _staffData.Aggregate()
                .Match(dd => dd.Timestamp < endTime && dd.Timestamp > startTime)
                .Group(x=> new { tagId=x.IndexId },g=>new { _id = g.Key, count = g.Count() } ).ToList();
            foreach (var d in data)
            {
                result.Add(d._id.tagId);
            }
            return result;
        }
        public List<string> GetTagIds(string deviceId, DateTime startTime, DateTime endTime)
        {
            List<string> result = new List<string>();
            var data = _staffData.Aggregate()
                .Match(dd => dd.Timestamp < endTime && dd.Timestamp > startTime && dd.DeviceId == deviceId)
                .Group(x => new { tagId = x.IndexId }, g => new { _id = g.Key, count = g.Count() }).ToList();
            foreach(var d in data)
            {
                result.Add(d._id.tagId);
            }
            return result;
        }
        /*按时间段deviceId、tagId、value统计出现次数*/
        public int GetCount(string deviceId,string tagId,int value,DateTime startTime,DateTime endTime)
        {
            var data = _staffData.Aggregate()
                .Match(dd => dd.Timestamp < endTime && dd.Timestamp > startTime && dd.DeviceId == deviceId && dd.IndexId == tagId && dd.IndexValue == value)
                .Group(x => new { deviceId = x.DeviceId, tagId = x.IndexValue, value = x.IndexValue },  g=>g.Count()).FirstOrDefault();
            return data;
        }
        /*获取一段时间内进出次数统计*/
        public List<RecordModel> GetCount(DateTime startTime,DateTime endTime)
        {
            List<RecordModel> result = new List<RecordModel>();
            var data=_staffData.Aggregate()
                .Match(dd => dd.Timestamp < endTime && dd.Timestamp > startTime)
                .Group(x => new { deviceId = x.DeviceId, deviceName=x.DeviceName,tagId = x.IndexId, value = x.IndexValue }, g =>
                            new { _id=g.Key, count = g.Count() }).ToList();
            foreach (var d in data)
            {
                result.Add(new RecordModel
                {
                    deviceName=d._id.deviceName,
                    deviceId = d._id.deviceId,
                    tagId = d._id.tagId,
                    value = d._id.value,
                    count = d.count
                });
            }
            return result;
        }
        /*获取indexId一段时间内的访问记录*/
        public List<RecordModel> GetByIndexId(string indexId, DateTime startTime, DateTime endTime)
        {
            List<RecordModel> result = new List<RecordModel>();
            var data = _staffData.AsQueryable()
                .Where(dd => dd.Timestamp < endTime && dd.Timestamp > startTime && dd.IndexId == indexId)
                .OrderByDescending(dd => dd.Timestamp)
                .Take(1000).ToList();
            foreach(var d in data)
            {
                result.Add(new RecordModel
                {
                    tagId = indexId,
                    deviceName = d.DeviceName,
                    deviceId = d.DeviceId,
                    timestamp = d.Timestamp.ToLocalTime(),
                    value = d.IndexValue
                });
            }
            return result;
        }
        /*获取deviceId的一段时间内的刷卡数据*/
        public List<RecordModel> GetCount(string deviceId,DateTime startTime,DateTime endTime)
        {
            List<RecordModel> result = new List<RecordModel>();
            var data = _staffData.Aggregate()
                .Match(dd => dd.Timestamp < endTime && dd.Timestamp > startTime && dd.DeviceId == deviceId)
                .Group(x => new { tagId = x.IndexId, value = x.IndexValue }, g => new { _id=g.Key,count=g.Count() }).ToList();
            foreach(var d in data)
            {
                result.Add(new RecordModel
                {
                    deviceId=deviceId,
                    tagId = d._id.tagId,
                    value = d._id.value,
                    count = d.count
                });
            }
            return result;
        }
        /*获取*/


        /*添加数据*/
        public void Create(StaffDataModel staffData)
        {
            _staffData.InsertOne(staffData);
        }

        /*按时间段统计staffId的考勤记录*/
        public void record()
        {
            
        }
    }
}
