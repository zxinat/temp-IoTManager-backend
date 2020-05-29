using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.IDao;
using IoTManager.Model;
using Microsoft.Extensions.Logging;
using StaffManager.Core.Infrastructures;
using StaffManager.Dao.Infrastructures;
using StaffManager.Models;
using StaffManager.Models.Serializers;
using StaffManager.Models.StatisticModels;

namespace StaffManager.Core
{
    public class StatisticBus:IStatisticBus
    {
        private readonly IStaffDao _staffDao;
        private readonly IStaffDataDao _staffData;
        private readonly ILogger _logger;
        private readonly ICustomerDao _customerDao;
        private readonly IDeviceDao _deviceDao;
        private readonly IRFIDTagDao _rFIDTagDao;
        private readonly IStaffAuthDao _staffAuthDao;
        private readonly IAttendenceDataDao _attendenceDataDao;
        public StatisticBus(IStaffDao staffDao,
            ILogger<StatisticBus> logger,
            IStaffDataDao staffData,
            IDeviceDao deviceDao,
            IStaffAuthDao staffAuthDao,
            IRFIDTagDao rFIDTagDao,
            ICustomerDao customerDao,
            IAttendenceDataDao attendenceDataDao)
        {
            _attendenceDataDao = attendenceDataDao;
            _staffDao = staffDao;
            _logger = logger;
            _staffData = staffData;
            _deviceDao = deviceDao;
            _staffAuthDao = staffAuthDao;
            _rFIDTagDao = rFIDTagDao;
            _customerDao = customerDao;
        }

        /*********************/
        /*    职工刷卡统计   */
        /********************/

        /* 1、车间进出人员显示，人数实时统计
         *  （1）最近进车间的10人；（2）最近离开车间的10人；（3）车间固定人员、当前人数、访客
         * 2、考勤统计：（1）车间考勤情况；（2）个人考勤情况
         * 3、职工踪迹查询：到访记录
         * 
         * 
         */


        /* 获取当前车间的员工和离开车间的员工*/
        /*
         * 输入：设备Id
         * step1 从MySQL staffauth中获取deviecId所有的属性Id（staffId）输出List<StaffModel>
         * step2 foreach(var staff in List<StaffModel>) 从MongoDB中获取当天最新的一条数据
         * step3 当天没有数据的Staff表示今天缺席，有数据时vaule=1表示在车间里，value=1表示离开车间
         * 输出：当前车间员工，离开车间的员工
         */
        public object GetCurrentStaffOnShop(string deviceId)
        {
            //List<StaffModel> staffs = _staffAuthDao.ListStaffsByDeviceId(deviceId);
            //List<StaffModel> inside = new List<StaffModel>();

            //List<StaffModel> outside = new List<StaffModel>();

            //List<StaffModel> absence = new List<StaffModel>();
            //List<StaffModel> customers = new List<StaffModel>();

            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //List<string> tagIds = _staffData.GetTagIds(today, today.AddDays(1));
            /*
             * V2
             */
            List<PersonSerializers> inside = new List<PersonSerializers>();
            List<PersonSerializers> outside = new List<PersonSerializers>();
            List<PersonSerializers> customers = new List<PersonSerializers>();
            //获取设备一天的所有数据
            List<StaffDataModel> staffDatas = _staffData.ListByDeviceId(deviceId, today, today.AddDays(1));
            //按标签Id分类
            Dictionary<string, List<TagRecordModel>> tagIdRecords = new Dictionary<string, List<TagRecordModel>>();
            foreach (var sd in staffDatas)
            {
                /*
                RFIDTagModel rfidTag = _rFIDTagDao.GetByTagId(sd.IndexId);
                PersonSerializers person = rfidTag != null ? (rfidTag.type == 1 ? new PersonSerializers(
                                  _staffDao.GetStaffInfoById(rfidTag.staffId)) : new PersonSerializers(
                                  _customerDao.GetCurrentByStaffId(rfidTag.staffId))) : null;
                */
                TagRecordModel tagRecord = new TagRecordModel
                {
                    tagId = sd.IndexId,
                    deviceId = sd.DeviceId,
                    timestamp = sd.Timestamp,
                    value = sd.IndexValue,
                    //person=person
                };
                if (!tagIdRecords.Keys.Contains(sd.IndexId))
                {
                    List<TagRecordModel> records = new List<TagRecordModel>();
                    records.Add(tagRecord);
                    tagIdRecords.Add(sd.IndexId, records);
                }
                else
                {
                    tagIdRecords[sd.IndexId].Add(tagRecord);
                }

            }

            foreach (var tagId in tagIdRecords.Keys)
            {

                RFIDTagModel rfidTag = _rFIDTagDao.GetByTagId(tagId);
                PersonSerializers person = rfidTag != null ? (rfidTag.type == 1 ? new PersonSerializers(
                                  _staffDao.GetStaffInfoById(rfidTag.staffId)) : new PersonSerializers(
                                  _customerDao.GetCurrentByStaffId(rfidTag.staffId))) : null;
                if (person != null)
                {
                    if (person.type == "访客")
                    {
                        customers.Add(person);
                    }
                    int enterCount = tagIdRecords[tagId].FindAll(d => d.value == 1).Count;
                    int exitCount = tagIdRecords[tagId].FindAll(d => d.value == 0).Count;
                    if (enterCount > exitCount)
                    {
                        inside.Add(person);
                    }
                    else if (enterCount <= exitCount)
                    {
                        outside.Add(person);
                    }

                }
                /*
                TagRecordModel record = tagIdRecords[tagId].FindAll(d => d.value == 1 & d.person.type == "访客").FirstOrDefault();
                if (record!=null)
                {
                    customers.Add(record.person);
                }
                if(tagIdRecords[tagId].FindAll(d=>d.value==1).Count> tagIdRecords[tagId].FindAll(d => d.value == 0).Count)
                {
                    inside.Add(tagIdRecords[tagId].Find(d => d.value == 1).person);
                }
                else
                {
                    outside.Add(tagIdRecords[tagId].Find(d => d.value == 0).person);
                }
                */
            }

            return new
            {
                inside = inside.Count(),
                insideStaffs = inside,
                outside = outside.Count(),
                outsideStaffs = outside,
                //total = total,
                //absence = absence.Count(),
                //absenceStaffs = absence,
                //fixedStaff = staffs.Count() - customers.Count(),
                customers = customers.Count(),
                customerStaffs = customers,
                description = "当前车间内人数：" + inside.Count() + " 人; " +
                                "当前离开车间人数: " + outside.Count() + " 人; " +
                                //"今日打卡总人数：" + total + " 人;" +
                                //"今日缺勤人数：" + absence.Count() + " 人;" +
                                // "固定人员：" + (staffs.Count() - customers.Count()) + " 人;" +
                                "外来人员：" + customers.Count() + " 人;"
            };
            //DateTime startTime = today;
            /*
            if (staffs.Count!=0)
            {
                foreach (var s in staffs)
                {
                    StaffDataModel deviceData = _staffData.ListByDeviceIdAndIndexId(deviceId, s.staffId, today, today.AddDays(1), 1).FirstOrDefault();
                    if (deviceData != null)
                    {
                        if(s.department=="访客")
                        {
                            customers.Add(s);
                        }
                        if (deviceData.IndexValue == 1)
                        {
                            inside.Add(s);
                        }
                        else if (deviceData.IndexValue == 0)
                        {
                            outside.Add(s);
                        }

                    }
                    else
                    {
                        if(s.department!="访客")
                        {
                            absence.Add(s);
                        }
                        
                    }
                }
                int total = inside.Count() + outside.Count();
                return new
                {
                    inside = inside.Count(),
                    insideStaffs=inside,
                    outside = outside.Count(),
                    outsideStaffs=outside,
                    total = total,
                    absence=absence.Count(),
                    absenceStaffs=absence,
                    fixedStaff=staffs.Count()-customers.Count(),
                    customers=customers.Count(),
                    customerStaffs=customers,
                    description="当前车间内人数："+inside.Count()+" 人; "+
                                "当前离开车间人数: "+outside.Count()+" 人; "+
                                "今日打卡总人数："+total+" 人;"+
                                "今日缺勤人数："+absence.Count()+ " 人;"+
                                "固定人员："+(staffs.Count()-customers.Count())+" 人;"+
                                "外来人员："+customers.Count()+" 人;"
                };
            }
            else
            {
                throw new Exception("No staff in This Device");
                //return "No staff in This Device";
            }
            */
        }
        /* 获取车间实时数据V3
         * 输入：deviceId
         * 输出：车间内人员数量，离开车间的人数，访客人数，打卡总人数
         * int inside、int outside、int customer、int total
         * step1、获取今日deviceId上所有的刷卡记录，tagId、value、count，进出次数
         * step2、轮询每个tagId、获取staffId以及type，
         * 
         */
        public object GetStatisticData(string deviceId)
        {
            int inside = 0;
            int outside = 0;
            int customer = 0;
            int total;
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            List<RecordModel> records = _staffData.GetCount(deviceId, today, today.AddDays(1));
            //total = (int)Math.Ceiling(records.Count() / 2.0);
            List<string> tagIds = _staffData.GetTagIds(deviceId, today, today.AddDays(1));
            total = tagIds.Count;
            foreach (var r in tagIds)
            {
                //标签r 进出次数统计
                RecordModel enter = records.Find(re => re.tagId == r && re.value == 1);
                RecordModel exit = records.Find(re => re.tagId == r && re.value == 0);
                int enterCount = enter != null ? enter.count : 0;
                int exitCount = exit != null ? exit.count : 0;
                if (enterCount > exitCount)
                {
                    inside++;
                }
                else
                {
                    outside++;
                }
                //判断是否为访客
                RFIDTagModel rFIDTag = _rFIDTagDao.GetByTagId(r);
                if (rFIDTag.type == 0)
                {
                    customer++;
                }
            }
            return new
            {
                inside = inside,
                outside = outside,
                customer = customer,
                total = total,
                description = "inside表示正在车间的人数，outside表示离开的人数，customer表示访客数，total表示今日在该设备上的打卡总数"
            };
        }

        /* 获取当日缺勤人数，固定人员总数
         * 输出：当日考勤情况，打卡总人数，固定人数，缺勤人数
         * int total、int staff、int absence
         * step1、获取所有正式工的staffId，以及相应tagId
         * step2、按时间、indexId查数据，没有数据则算缺勤
         */
        public object GetAttendenceRecords(DataStatisticRequestModel date)
        {
            int total = 0;//打卡总人数
            int staff = 0;//固定人数
            int absence = 0;//缺勤人数
            //DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime sTime = date.StartTime.ToLocalTime().Date;
            DateTime eTime = date.EndTime.ToLocalTime().Date.AddDays(1);
            List<string> tagIds = _staffData.GetTagIds(sTime, eTime);
            total = tagIds.Count;
            List<string> staffIds = _staffDao.ListStaffIds("staff");
            staff = staffIds.Count();
            foreach (var s in staffIds)
            {
                RFIDTagModel rFIDTag = _rFIDTagDao.ListByStaffId(s).Find(rr => rr.status == 1);
                StaffDataModel staffData = rFIDTag != null ? _staffData.ListByIndexId(rFIDTag.tagId, sTime, eTime, 1).FirstOrDefault() : null;
                if (staffData == null)
                {
                    absence++;
                }
            }
            return new
            {
                staff = staff,
                total = total,
                absence = absence,
                descriptions = $"时间段：{sTime.ToShortDateString()} - {eTime.ToShortDateString()}，staff表示固定人员数，total表示打卡总数，absence表示缺勤人数"
            };
        }
        /* 获取车间昨日考勤情况
         */
        
        /* 获取车间A的考勤情况
         */
        public object GetAttendenceRecords(string deviceId, DataStatisticRequestModel date)
        {
            int total = 0;//实到人数
            int staff = 0;//应到人数
            int absence = 0;//缺勤人数
            DateTime sTime = date.StartTime.ToLocalTime().Date;
            DateTime eTime = date.EndTime.ToLocalTime().Date.AddDays(1);
            // DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //获取当日在deviceId上打卡的tagId
            List<string> tagIds = _staffData.GetTagIds(deviceId, sTime, eTime);
            //获取deviceId上授权的staffId
            List<string> staffIds = _staffAuthDao.ListStaffIdsByDeviceId(deviceId, sTime, eTime);
            staff = staffIds.Count();
            foreach(var sid in staffIds)
            {
                RFIDTagModel rFIDTag = _rFIDTagDao.GetByStaffId(sid);
                if(rFIDTag!=null)
                {
                    if(tagIds.Contains(rFIDTag.tagId))
                    {
                        total++;
                    }
                    else
                    {
                        absence++;
                    }
                }
            }
            return new
            {
                staff = staff,
                total = total,
                absence = absence,
                descriptions = $"时间段：{sTime}-{eTime}，staff表示车间{deviceId}固定人员数，total表示出勤人数，absence表示缺勤人数"
            };
        }
        /*获取所有门禁设备的*/
        /*获取车间A一段时间的访问记录*/
        public Dictionary<string, List<TagRecordModel>> GetTagRecordsBydeviceId(string deviceId, DateTime startTime, DateTime endTime)
        {
            List<StaffDataModel> staffDatas = _staffData.ListByDeviceId(deviceId, startTime, endTime);
            //按标签Id分类
            Dictionary<string, List<TagRecordModel>> tagIdRecords = new Dictionary<string, List<TagRecordModel>>();
            List<string> tagIds = new List<string>();
            foreach (var sd in staffDatas)
            {
                RFIDTagModel rfidTag = _rFIDTagDao.GetByTagId(sd.IndexId);
                PersonSerializers person = rfidTag != null ? (rfidTag.type == 1 ? new PersonSerializers(
                                  _staffDao.GetStaffInfoById(rfidTag.staffId)) : new PersonSerializers(
                                  _customerDao.GetCurrentByStaffId(rfidTag.staffId))) : null;
                TagRecordModel tagRecord = new TagRecordModel
                {
                    tagId = sd.IndexId,
                    deviceId = sd.DeviceId,
                    timestamp = sd.Timestamp,
                    value = sd.IndexValue,
                    person = person
                };
                if (!tagIdRecords.Keys.Contains(sd.IndexId))
                {
                    List<TagRecordModel> records = new List<TagRecordModel>();
                    records.Add(tagRecord);
                    tagIdRecords.Add(sd.IndexId, records);
                }
                else
                {
                    tagIdRecords[sd.IndexId].Add(tagRecord);
                }

            }
            return tagIdRecords;
        }
        /*获取昨日车间员工进出情况*/
        public object GetYesterdayStaffOnShop(string deviceId)
        {
            List<StaffModel> staffs = _staffAuthDao.ListStaffsByDeviceId(deviceId);
            List<StaffModel> inside = new List<StaffModel>();
            List<StaffModel> outside = new List<StaffModel>();
            List<StaffModel> absence = new List<StaffModel>();
            List<StaffModel> customers = new List<StaffModel>();
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime startTime = today.AddDays(-1);
            if (staffs.Count != 0)
            {
                foreach (var s in staffs)
                {

                    StaffDataModel deviceData = _staffData.ListByDeviceIdAndIndexId(deviceId, s.staffId, startTime, startTime.AddDays(1), 2).FirstOrDefault();
                    if (deviceData != null)
                    {
                        if (s.department == "访客")
                        {
                            customers.Add(s);
                        }
                        if (deviceData.IndexValue == 1)
                        {
                            inside.Add(s);
                        }
                        else if (deviceData.IndexValue == 0)
                        {
                            outside.Add(s);
                        }
                    }
                    else
                    {
                        if (s.department != "访客")
                        {
                            absence.Add(s);
                        }

                    }
                }
                int total = inside.Count() + outside.Count();
                return new
                {
                    //inside = inside.Count(),
                    //insideStaffs = inside,
                    //outside = outside.Count(),
                    //outsideStaffs = outside,
                    total = total,
                    totalStaffs = inside.Union(outside),
                    absence = absence.Count(),
                    absenceStaffs = absence,
                    fixedStaff = staffs.Count() - customers.Count(),
                    customers = customers.Count(),
                    customerStaffs = customers,
                    description =
                                "昨日打卡总人数：" + total + " 人;" +
                                "昨日缺勤人数：" + absence.Count() + " 人;" +
                                "固定人员：" + (staffs.Count() - customers.Count()) + " 人;" +
                                "外来人员：" + customers.Count() + " 人;"
                };
            }
            else
            {
                throw new Exception("No staff in This Device");
                //return "No staff in This Device";
            }
        }
        /*获取员工考勤一段时间内的记录*/
        public object GetAttendenceRecord(string staffId, DateTime startTime, DateTime endTime)
        {
            List<string> devices = _staffAuthDao.GetAuthDevice(staffId);
            Dictionary<string, List<StaffDataModel>> records = new Dictionary<string, List<StaffDataModel>>();
            foreach (var d in devices)
            {
                List<StaffDataModel> datas = _staffData.ListByDeviceIdAndIndexId(d, staffId, startTime, endTime);
                records.Add(d, datas);
            }
            return null;
        }
        /*获取员工一段时间内的刷卡记录（在所有设备上）*/


        /*最近入场的10人*/
        public object ListLatestEnter10(string deviceId)
        {
            try
            {
                List<StaffDataModel> datas = _staffData.ListByDeviceIdAndValue(deviceId, 1, 10);
                List<object> result = new List<object>();
                if (datas.Count != 0)
                {

                    foreach (var d in datas)
                    {
                        //通过标签Id获取staffId
                        RFIDTagModel rfidTag = _rFIDTagDao.GetByTagId(d.IndexId);
                        if (rfidTag != null)
                        {
                            PersonSerializers person = rfidTag.type == 1 ? new PersonSerializers(
                                _staffDao.GetStaffInfoById(rfidTag.staffId)) : new PersonSerializers(
                                _customerDao.GetCurrentByStaffId(rfidTag.staffId));
                            result.Add(new
                            {
                                staff = person,
                                deviceId = d.DeviceId,
                                deviceName = d.DeviceName,
                                timeStamp = d.Timestamp,

                            });
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        /*获取最近离场的10人*/
        public object ListLatestExit10(string deviceId)
        {
            try
            {
                List<StaffDataModel> datas = _staffData.ListByDeviceIdAndValue(deviceId, 0, 10);
                List<object> result = new List<object>();
                if (datas.Count != 0)
                {

                    foreach (var d in datas)
                    {
                        //通过标签Id获取staffId
                        RFIDTagModel rfidTag = _rFIDTagDao.GetByTagId(d.IndexId);
                        if (rfidTag != null)
                        {
                            PersonSerializers person = rfidTag.type == 1 ? new PersonSerializers(
                                _staffDao.GetStaffInfoById(rfidTag.staffId)) : new PersonSerializers(
                                _customerDao.GetCurrentByStaffId(rfidTag.staffId));
                            result.Add(new
                            {
                                staff = person,
                                deviceId = d.DeviceId,
                                deviceName = d.DeviceName,
                                timeStamp = d.Timestamp,

                            });
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        /* 获取车间deviceId离场的所有人以及离场计时
         * 输入：DeviceId，输出：离岗人员staffId，Name，离岗时刻，离岗计时
         * step1、获取当日deviceId所有index=0的所有tagId和timestamp
         * step2、通过tagId获取staffId、name，
         */
        public object GetExitInfo(string deviceId)
        {
            List<ExitInfoModel> exitInfos = _staffData.ListExitInfo(deviceId);
            List<ExitInfoSerializer> result = new List<ExitInfoSerializer>();
            DateTime today = DateTime.Now;
            DateTime sTime = today.Date;
            foreach( var e in exitInfos)
            {
                RFIDTagModel rFIDTags = _rFIDTagDao.GetByTagId(e.tagId);
                e.staffId = rFIDTags.staffId;
                if(rFIDTags.type==1)
                {
                    StaffModel staff = _staffDao.GetStaffInfoById(rFIDTags.staffId);
                    e.name = staff.staffName;
                    result.Add(new ExitInfoSerializer(e));
                }
            }
            return result;

        }
        /*最新刷卡员工*/
        public object GetLatestOne(string deviceId)
        {
            try
            {
                StaffDataModel latestData = _staffData.GetLastDataByDeviceId(deviceId);
                StaffModel staff = new StaffModel();
                if (latestData != null)
                {
                    RFIDTagModel rfidTag = _rFIDTagDao.GetByTagId(latestData.IndexId);
                    //staff = _staffDao.GetStaffInfoById(latestData.IndexId);
                    if (rfidTag != null)
                    {
                        PersonSerializers person = rfidTag.type == 1 ? new PersonSerializers(
                            _staffDao.GetStaffInfoById(rfidTag.staffId)) : new PersonSerializers(
                            _customerDao.GetCurrentByStaffId(rfidTag.staffId));
                        return new
                        {
                            staff = person,
                            timeStamp = latestData.Timestamp,
                            status = latestData.IndexValue == 0 ? "离场" : "入场",
                            deviceId = deviceId
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        /* 人员踪迹查询
         * 1、人员当前所在车间
         * 2、人员在一段时间内到访的地点：输入时间，staffId；输出：staffId，List<deviceId，tagId，value，timestamp>
         */
         public PersonalRecordModel GetCurrentData(string staffId)
        {
            RFIDTagModel rFIDTag = _rFIDTagDao.GetByStaffId(staffId);
            StaffModel staff = _staffDao.GetStaffInfoById(staffId);
            StaffDataModel data = rFIDTag != null ? _staffData.GetByIndexIdLatestOne(rFIDTag.tagId)
                : null;
            PersonalRecordModel record = new PersonalRecordModel
            {
                deviceId = data.DeviceId,
                tagId = data.IndexId,
                deviceName = data.DeviceName,
                value = data.IndexValue,
                timestamp = data.Timestamp.ToLocalTime(),
                staffId = staffId,
                name = staff.staffName
            };
            return record;
        }
        /*查看人员到访记录*/
        public PersonalRecordsModel GetPersonalRecord(string staffId, DataStatisticRequestModel date)
        {
            PersonalRecordsModel personalRecords = new PersonalRecordsModel();
            List<RFIDTagModel> rFIDTags = _rFIDTagDao.ListByStaffId(staffId, date.StartTime, date.EndTime);
            foreach(var r in rFIDTags)
            {
                List<RecordModel> records = _staffData.GetByIndexId(r.tagId, date.StartTime, date.EndTime);
                if (r.type==1)
                {
                    StaffModel staff = _staffDao.GetStaffInfoById(r.staffId);
                    personalRecords.staffId = staffId;
                    personalRecords.name = staff.staffName;
                    personalRecords.affiliation = staff.department;
                    personalRecords.records = personalRecords.records != null ? records.Union(personalRecords.records).ToList()
                        : records;
                }
                else
                {
                    CustomerModel customer = _customerDao.GetCurrentByStaffId(r.staffId);
                    personalRecords.staffId = staffId;
                    personalRecords.name = customer.name;
                    personalRecords.affiliation = customer.affiliation;
                    personalRecords.records = personalRecords.records != null ? records.Union(personalRecords.records).ToList()
                        : records;
                }
            }
            return personalRecords;
                     
        }



        /* 考勤统计
         */
        /*个人考勤统计*/
        public object GetPersonalAttendenceData(string staffId, DataStatisticRequestModel date, string statisticType = "week")
        {
            return _attendenceDataDao.GetPersonalStatisticData(staffId,date.StartTime,date.EndTime, statisticType);
        }

        /*部门考勤统计*/
        public object GetDepartmentAttendenceData(DataStatisticRequestModel date,string statisticType = "week")
        {
            return _attendenceDataDao.GetDepartmentAttendenceData(date.StartTime, date.EndTime, statisticType);
        }

        /* 离岗人员以及计时统计
         */
        









        /*模拟员工打卡使用*/
        /*查看员工当日最新的打卡状态 0表示当日打过卡，且已离开；1表示当前还在车间，-1表示今日未打卡*/
        public int GetCurrentStatusByStaffId(string deviceId, string staffId)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            StaffDataModel deviceData = _staffData.ListByDeviceIdAndIndexId(deviceId, staffId, today, today.AddDays(1), 1).FirstOrDefault();
            int status;
            if (deviceData != null)
            {
                status = deviceData.IndexValue == 0 ? 0 : 1;
            }
            else
            {
                status = -1;
            }
            return status;
        }
    }
}
