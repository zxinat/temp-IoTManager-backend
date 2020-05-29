using Microsoft.Extensions.Logging;
using StaffManager.Core.Jobs.Infrastructures;
using StaffManager.Dao.Infrastructures;
using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Core.Jobs
{
    public class AttendenceRecordJob:IAttendenceRecordJob
    {
        private readonly IAttendenceDataDao _attendenceData;
        private readonly IRFIDTagDao _rFIDTagDao;
        private readonly IStaffDao _staffDao;
        private readonly IStaffDataDao _staffDataDao;
        private readonly ILogger _logger;
        public AttendenceRecordJob(IAttendenceDataDao attendenceData,
            ILogger<AttendenceRecordJob> logger,
            IStaffDao staffDao,
            IStaffDataDao staffDataDao,
            IRFIDTagDao rFIDTagDao)
        {
            _staffDao = staffDao;
            _staffDataDao = staffDataDao;
            _attendenceData = attendenceData;
            _logger = logger;
            _rFIDTagDao = rFIDTagDao;
        }

        public void Run()
        {
            _logger.LogInformation("每日考勤记录统计...");
            //获取所有正式员工
            List<string> staffIds = _staffDao.ListStaffIds("staff");
            DateTime now = DateTime.Now;
            DateTime today = new DateTime(now.Year, now.Month, now.Day);
            //获取今日数刷卡的标签列表
            List<string> tagIds = _staffDataDao.GetTagIds(today, today.AddDays(1));
            List<AttendenceDataModel> attendenceDatas = new List<AttendenceDataModel>();
            List<AttendenceDataModel> updateDatas = new List<AttendenceDataModel>();
            
            //获取tagId对应的staffId
            foreach(var sid in staffIds)
            {
                RFIDTagModel rFIDTag = _rFIDTagDao.GetByStaffId(sid);
                if(rFIDTag!=null)
                {
                    if (tagIds.Contains(rFIDTag.tagId))
                    {
                        //查重处理 日期（天）不可重复

                        var ad = new AttendenceDataModel
                        {
                            staffId = sid,
                            tagId = rFIDTag.tagId,
                            value = 1,
                            timestamp = DateTime.Now
                        };
                        if(!_attendenceData.IsContain(ad))
                        {
                            attendenceDatas.Add(ad);
                        }
                        else
                        {
                            updateDatas.Add(ad);
                        }
                    }
                    else
                    {
                        var ad = new AttendenceDataModel
                        {
                            staffId = sid,
                            tagId = rFIDTag.tagId,
                            value = 0,
                            timestamp = DateTime.Now
                        };
                        if (!_attendenceData.IsContain(ad))
                        {
                            attendenceDatas.Add(ad);
                        }
                        else
                        {
                            updateDatas.Add(ad);
                        }
                    }
                }
                
            }
            _attendenceData.BatchAddAsync(attendenceDatas);
            _attendenceData.BatchUpdateAsync(updateDatas);
        }
    }
}
