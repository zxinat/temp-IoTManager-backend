using IoTManager.Utility;
using MongoDB.Driver;
using StaffManager.Dao.Infrastructures;
using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using StaffManager.Models.StatisticModels;
using Microsoft.Extensions.Logging;

namespace StaffManager.Dao
{
    public class AttendenceDataDao:IAttendenceDataDao
    {
        private readonly IMongoCollection<AttendenceDataModel> _attendenceData;
        private readonly IStaffDataDao _staffDataDao;
        private readonly IStaffDao _staffDao;
        private readonly IDepartmentDao _departmentDao;
        private readonly ILogger _logger;
        public AttendenceDataDao(ILogger<AttendenceDataDao> logger,
            IDepartmentDao departmentDao,
            IStaffDao staffDao,
            IStaffDataDao staffDataDao)
        {
            _staffDataDao = staffDataDao;
            _staffDao = staffDao;
            _logger = logger;
            _departmentDao = departmentDao;
            var client = new MongoClient(Constant.getMongoDBConnectionString());
            var database = client.GetDatabase("iotmanager");
            _attendenceData = database.GetCollection<AttendenceDataModel>("AttendenceRecord");
        }
        public void Add(AttendenceDataModel attendenceData)
        {
             _attendenceData.InsertOne(attendenceData);
        }
        public DeleteResult RemoveByStaffId(string staffId)
        {
            var filter = Builders<AttendenceDataModel>.Filter.Eq("StaffId", staffId);
            return _attendenceData.DeleteMany(filter);
        }
        public async Task BatchAddAsync(List<AttendenceDataModel> attendenceDatas)
        {
            await _attendenceData.InsertManyAsync(attendenceDatas);
        }
        /*批量更新*/
        public async Task<BulkWriteResult<AttendenceDataModel>> BatchUpdateAsync(List<AttendenceDataModel> attendenceDatas)
        {
            List<UpdateOneModel<AttendenceDataModel>> newDocs = new List<UpdateOneModel<AttendenceDataModel>>();
            FilterDefinitionBuilder<AttendenceDataModel> builder = Builders<AttendenceDataModel>.Filter;
            foreach(var ad in attendenceDatas)
            {
                DateTime startTime = ad.timestamp.Date.ToUniversalTime();
                var filter = builder.And(
                    builder.Eq("StaffId",ad.staffId),
                    builder.Eq("TagId",ad.tagId),
                    builder.Gt("Timestamp",startTime),
                    builder.Lt("Timestamp",startTime.AddDays(1)));
                var update = Builders<AttendenceDataModel>.Update.Set("Value", ad.value).Set("Timestamp", DateTime.UtcNow);
                var uad = new UpdateOneModel<AttendenceDataModel>(filter, update)
                {
                    IsUpsert = true
                };
                newDocs.Add(uad);
            }
             return await _attendenceData.BulkWriteAsync(newDocs);
        }
        /* 数据查重
         */
        public bool IsContain(AttendenceDataModel ad)
        {
            DateTime startTime = ad.timestamp.Date.ToUniversalTime();
            var query = _attendenceData.AsQueryable()
                .Where(dd => dd.staffId == ad.staffId & dd.tagId == ad.tagId & dd.value == ad.value &
                dd.timestamp > startTime & dd.timestamp < startTime.AddDays(1)).ToList();
            return query.Count == 0 ? false : true;            
        }

        /* 统计聚合：
         * 缺勤定义：指定正常上班日期
         * 1、按周统计个人考勤情况：每周出勤几天，缺勤的日期，天数
         * 2、按月统计个人考勤情况：每月出勤天数，缺勤天数，缺勤日期列表
         * 3、按周统计每个部门考勤情况：部门缺勤总次数，出勤率
         * 4、按月统计每个部门考勤情况：缺勤总次数，出勤率
         */
        //按周统计个人出勤情况
        //输入时间段，staffId；输出出勤天数，缺勤天数，具体的缺勤日期
        public List<PersonalAttendenceModel> GetPersonalStatisticData(string staffId,DateTime startTime,DateTime endTime,string statisticType="week")
        {
            List<PersonalAttendenceModel> result = new List<PersonalAttendenceModel>();
            if(statisticType=="week")
            {
                result = GetPersonalAttendencesWeekData(staffId, startTime, endTime);
            }
            else if(statisticType=="month")
            {
                result = GetPersonalAttendencesMonthData(staffId, startTime, endTime);
            }
            else
            {
                throw new Exception("statisticType值错误，Input: week(按周统计)、month(按月统计)");
            }
            return result;
        }

        public List<PersonalAttendenceModel> GetPersonalAttendencesWeekData(string staffId, DateTime startTime, DateTime endTime)
        {
            GregorianCalendar calendar = new GregorianCalendar();
            int startWeek = calendar.GetWeekOfYear(startTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            int endWeek = calendar.GetWeekOfYear(endTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            int totalWeek = endWeek - startWeek + 1;
            DateTime temp = new DateTime(startTime.Year, 1, 1);
            int dayCount = (int)temp.DayOfWeek;
            //DateTime temp2= new DateTime(startTime.Year, 1, 6);
            //int num = calendar.GetWeekOfYear(temp, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            //int num2= calendar.GetWeekOfYear(temp2, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            List<PersonalAttendenceModel> result = new List<PersonalAttendenceModel>();
            for (int i = startWeek; i <= endWeek; i++)
            {
                DateTime sTime = new DateTime();
                DateTime eTime = new DateTime();
                if (i == 1)
                {
                    sTime = temp.ToUniversalTime();
                    eTime = temp.AddDays(7 - dayCount + 1).ToUniversalTime();
                }
                else
                {
                    sTime = temp.AddDays(7 * (i - 1) - dayCount + 1).ToUniversalTime();
                    eTime = sTime.AddDays(7).ToUniversalTime();
                }
                PersonalAttendenceModel personalAttendence = new PersonalAttendenceModel();
                personalAttendence.id = i;
                personalAttendence.absenceDates = GetAbsenceDates(staffId, sTime, eTime);
                personalAttendence.missNum = GetCount(staffId, sTime, eTime, 0);
                personalAttendence.workNum = GetCount(staffId, sTime, eTime, 1);
                personalAttendence.remark = $"第 {i} 周";
                personalAttendence.startTime = sTime.ToLocalTime();
                personalAttendence.endTime = eTime.ToLocalTime();
                result.Add(personalAttendence);
            }
            return result;
        }
        //按月统计个人考勤情况
        public List<PersonalAttendenceModel> GetPersonalAttendencesMonthData(string staffId ,DateTime startTime,DateTime endTime)
        {
            GregorianCalendar calendar = new GregorianCalendar();
            int startMonth = calendar.GetMonth(startTime);
            int endMonth = calendar.GetMonth(endTime);
            List<PersonalAttendenceModel> result = new List<PersonalAttendenceModel>();
            for(int m=startMonth;m<=endMonth;m++)
            {
                DateTime sTime = new DateTime(startTime.Year, m, 1).ToUniversalTime();
                DateTime eTime = sTime.AddMonths(1);
                PersonalAttendenceModel personalAttendence = new PersonalAttendenceModel();
                personalAttendence.id = m;
                personalAttendence.absenceDates = GetAbsenceDates(staffId, sTime, eTime);
                personalAttendence.missNum = GetCount(staffId, sTime, eTime, 0);
                personalAttendence.workNum = GetCount(staffId, sTime, eTime, 1);
                personalAttendence.remark = $"{sTime.Year}年 {m}月";
                personalAttendence.startTime = sTime.ToLocalTime();
                personalAttendence.endTime = eTime.ToLocalTime();
                result.Add(personalAttendence);
            }
            return result;
        }
        //获取缺勤/出勤次数
        public int GetCount(string staffId, DateTime startTime, DateTime endTime, int value)
        {
            return _attendenceData.AsQueryable()
                .Where(x => x.staffId == staffId && x.timestamp > startTime && x.timestamp < endTime && x.value==value).Count();
        }
        //获取缺勤日期列表
        public List<string> GetAbsenceDates(string staffId ,DateTime startTime,DateTime endTime)
        {
            List<string> result = new List<string>();
            var data = _attendenceData.Aggregate()
                .Match(dd => dd.staffId == staffId && dd.timestamp < endTime && dd.timestamp > startTime && dd.value == 0)
                .Group(x => new { timestamp = x.timestamp }, g => new { _id = g.Key, count = g.Count() }).ToList();
            foreach( var d in data)
            {
                DateTime date = d._id.timestamp.ToLocalTime();
                string re = date.Year.ToString() + " " + date.Month.ToString() + "-" + date.Day.ToString();
                result.Add(re);
            }
            return result;
        }
        public List<DepartmentAttendenceModel> GetDepartmentAttendenceData(DateTime startTime, DateTime endTime,string statisticType="week")
        {
            List<DepartmentAttendenceModel> result = new List<DepartmentAttendenceModel>();
            if (statisticType == "week")
            {
                result = GetDepartmentAttendenceWeekData(startTime, endTime);
            }
            else if (statisticType == "month")
            {
                result = GetDepartmentAttendenceMonthData(startTime, endTime);
            }
            else
            {
                throw new Exception("statisticType值错误，Input: week(按周统计)、month(按月统计)");
            }
            return result;
        }

        //按周统计部门考勤情况
        public List<DepartmentAttendenceModel> GetDepartmentAttendenceWeekData(DateTime startTime,DateTime endTime)
        {
            GregorianCalendar calendar = new GregorianCalendar();
            int startWeek = calendar.GetWeekOfYear(startTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            int endWeek = calendar.GetWeekOfYear(endTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            int totalWeek = endWeek - startWeek + 1;
            DateTime temp = new DateTime(startTime.Year, 1, 1);
            int dayCount = (int)temp.DayOfWeek;
            List<DepartmentAttendenceModel> result = new List<DepartmentAttendenceModel>();
            for (int i = startWeek; i <= endWeek; i++)
            {
                DateTime sTime = new DateTime();
                DateTime eTime = new DateTime();
                if (i == 1)
                {
                    sTime = temp.ToUniversalTime();
                    eTime = temp.AddDays(7 - dayCount + 1).ToUniversalTime();
                }
                else
                {
                    sTime = temp.AddDays(7 * (i - 1) - dayCount + 1).ToUniversalTime();
                    eTime = sTime.AddDays(7).ToUniversalTime();
                }
                DepartmentAttendenceModel departmentAttendence = new DepartmentAttendenceModel();
                List<DepartStatisticModel> departStatistics = new List<DepartStatisticModel>();
                //获取本周部门列表
                List<DepartmentModel> departments = _departmentDao.ListByDate(sTime, eTime);
                foreach(var d in departments)
                {
                    DepartStatisticModel departStatistic = new DepartStatisticModel();
                    departStatistic.id = d.id;
                    departStatistic.name = d.departmentName;
                    //获取该部门时间段内的员工
                    List<string> staffIds = _staffDao.ListStaffIdsByDepartmentAndDate(d.departmentName, sTime, eTime);
                    departStatistic.staffNum = staffIds.Count();
                    //获取这段时间打卡的staffId
                    List<string> workStaffIds = ListStaffIdsByDate(sTime, eTime, 1);
                    List<string> attendStaffIds= staffIds.Except(workStaffIds).ToList();
                    departStatistic.attendenceStaffIds = attendStaffIds;
                    departStatistic.missNum = attendStaffIds.Count();
                    departStatistic.rate = staffIds.Count() != 0 ? Math.Round(( (Double)(staffIds.Count() - attendStaffIds.Count()) / staffIds.Count()),4)
                        : 0;
                    departStatistics.Add(departStatistic);
                }
                departmentAttendence.departStatistics = departStatistics;
                departmentAttendence.id = i;
                departmentAttendence.remark= $"第 {i} 周";
                departmentAttendence.startTime = sTime.ToLocalTime();
                departmentAttendence.endTime = eTime.ToLocalTime();
                result.Add(departmentAttendence);
            }
            return result;
        }

        //按月统计部门考勤情况
        public List<DepartmentAttendenceModel> GetDepartmentAttendenceMonthData(DateTime startTime, DateTime endTime)
        {
            GregorianCalendar calendar = new GregorianCalendar();
            int startMonth = calendar.GetMonth(startTime);
            int endMonth = calendar.GetMonth(endTime);
            List<DepartmentAttendenceModel> result = new List<DepartmentAttendenceModel>();
            for (int m = startMonth; m <= endMonth; m++)
            {
                DateTime sTime = new DateTime(startTime.Year, m, 1).ToUniversalTime();
                DateTime eTime = sTime.AddMonths(1);
                DepartmentAttendenceModel departmentAttendence = new DepartmentAttendenceModel();
                List<DepartStatisticModel> departStatistics = new List<DepartStatisticModel>();
                //获取本周部门列表
                List<DepartmentModel> departments = _departmentDao.ListByDate(sTime, eTime);
                foreach (var d in departments)
                {
                    DepartStatisticModel departStatistic = new DepartStatisticModel();
                    departStatistic.id = d.id;
                    departStatistic.name = d.departmentName;
                    //获取该部门时间段内的员工
                    List<string> staffIds = _staffDao.ListStaffIdsByDepartmentAndDate(d.departmentName, sTime, eTime);
                    departStatistic.staffNum = staffIds.Count();
                    //获取这段时间打卡的staffId
                    List<string> workStaffIds = ListStaffIdsByDate(sTime, eTime, 1);
                    List<string> attendStaffIds = staffIds.Except(workStaffIds).ToList();
                    departStatistic.attendenceStaffIds = attendStaffIds;
                    departStatistic.missNum = attendStaffIds.Count();
                    departStatistic.rate = staffIds.Count() != 0 ? Math.Round(((Double)(staffIds.Count() - attendStaffIds.Count()) / staffIds.Count()), 4)
                        : 0;
                    departStatistics.Add(departStatistic);
                }
                departmentAttendence.departStatistics = departStatistics;
                departmentAttendence.id = m;
                departmentAttendence.remark = $"{sTime.Year}年 {m}月";
                departmentAttendence.startTime = sTime.ToLocalTime();
                departmentAttendence.endTime = eTime.ToLocalTime();
                result.Add(departmentAttendence);
            }
            return result;
        }
        /*统计一段时间出勤/缺勤的staffId*/
        public List<string> ListStaffIdsByDate(DateTime startTime,DateTime endTime,int value)
        {
            List<string> result = new List<string>();
            var data = _attendenceData.Aggregate()
                .Match(dd => dd.timestamp < endTime && dd.timestamp > startTime && dd.value == value)
                .Group(x => new { staffId = x.staffId }, g => new { _id = g.Key, count = g.Count() }).ToList();
            foreach(var d in data)
            {
                result.Add(d._id.staffId);
            }
            return result;
        }
    }
}
