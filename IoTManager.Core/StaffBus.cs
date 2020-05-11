using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace IoTManager.Core
{
    public class StaffBus:IStaffBus
    {
        private readonly IStaffDao _staffDao;
        private readonly ILogger _logger;
        private readonly IDeviceDataDao _deviceDataDao;
        private readonly IDeviceDao _deviceDao;
        /*构造函数注入*/
        public StaffBus(IStaffDao staffDao,ILogger<StaffBus> logger,IDeviceDataDao deviceDataDao, IDeviceDao deviceDao)
        {
            _staffDao = staffDao;
            _logger = logger;
            _deviceDataDao = deviceDataDao;
            _deviceDao = deviceDao;
        }
        /*
         * 获取所有员工信息，
         * 输入：
         *  search 搜索类型：search="search"时，开启分页功能，
         *           否则后面的参数都无效，返回所有员工
         *  page 页码
         *  sortColumn 排序字段
         *  order 升序(asc)/降序(dsc)
         *  部门
         *  
         * 输出：
         * 员工列表
         */
         
        public object ListStaffs(string search, int page, string sortColumn, string order, string department)
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            try
            {
                List<StaffModel> staffs = _staffDao.ListStaffs(search, department, offset, limit, sortColumn, order);
                List<StaffSerializer> result = new List<StaffSerializer>();
                foreach (var s in staffs)
                {
                    result.Add(new StaffSerializer(s));
                }
                return result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }   
        }
        /*通过工号获取员工信息*/
        public object GetByStaffId(string staffId)
        {
            try
            {
                return _staffDao.GetStaffInfoById(staffId);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*创建员工*/
        public string Create(StaffSerializer staffSerializer)
        {
            StaffModel staff = new StaffModel
            {
                staffId = staffSerializer.staffId,
                staffName = staffSerializer.staffName,
                gender = staffSerializer.gender,
                age = staffSerializer.age,
                department = staffSerializer.department,
                phoneNumber = staffSerializer.phoneNumber,
                email = staffSerializer.email,
                remark = staffSerializer.remark
            };
            try
            {
                StaffModel eStaff = _staffDao.GetStaffInfoById(staffSerializer.staffId);
                if (eStaff == null)
                {
                    return _staffDao.Create(staff);
                }
                else
                {

                    return "exist";
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
            

        }
        /*删除员工*/
        public string Delete(string staffId)
        {
            try
            {
                return  _staffDao.Delete(staffId);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*批量删除员工*/
        public object BatchDelete(List<string> staffIds)
        {
            try
            {
                return _staffDao.BatchDelete(staffIds);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*修改员工信息*/
        public string Update(string staffId,StaffSerializer staffSerializer)
        {
            try
            {
                StaffModel staff=_staffDao.GetStaffInfoById(staffId);
                staff.staffName = staffSerializer.staffName;
                staff.gender = staffSerializer.gender;
                staff.age = staffSerializer.age;
                staff.department = staffSerializer.department;
                staff.phoneNumber = staffSerializer.phoneNumber;
                staff.email = staffSerializer.email;
                staff.remark = staffSerializer.remark;
                return _staffDao.Update(staffId, staff);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*上传照片*/
        public string UpLoadImage(string staffId,string base64Image)
        {
            try
            {
                StaffModel staff = _staffDao.GetStaffInfoById(staffId);
                staff.base64Image = base64Image;
                return _staffDao.Update(staffId, staff);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        /*添加员工权限*/
        public string AddAuth(StaffAuthModel staffAuth)
        {
            try
            {
                if(!_staffDao.Contain(staffAuth.staffId,staffAuth.deviceId))
                {
                    return _staffDao.AddAuth(staffAuth);
                }
                else
                {
                    return "exist";
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*批量给员工添加权限*/
        public object BatchAddAuth(string staffId,List<string> deviceIds)
        {
            try
            {
                int rows = 0;
                foreach(var did in deviceIds)
                {
                    if(!_staffDao.Contain(staffId, did))
                    {
                        StaffAuthModel staffAuth = new StaffAuthModel
                        {
                            staffId = staffId,
                            deviceId = did
                        };
                        _staffDao.AddAuth(staffAuth);
                        rows = rows + 1;
                    }
                }
                return rows;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*列出授权的设备*/
        public object GetAuthDevice(string staffId)
        {
            try
            {
                return _staffDao.GetAuthDevice(staffId);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*删除员工的权限*/
        public string DeleteAuth(string staffId,string deviceId)
        {
            try
            {
                return _staffDao.DeleteAuth(staffId, deviceId);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*批量删除员工权限*/
        public object BatchDeleteAuth(string staffId,List<string> deviceIds )
        {
            try
            {
                return _staffDao.BatchDeleteAuth(staffId, deviceIds);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        /*获取设备授权的工号*/
        public object ListStaffIdsByDeviceId(string deviceId)
        {
            try
            {
                return _staffDao.ListStaffIdsByDeviceId(deviceId);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*按部门分配门禁权限*/
        public object AddAuthByDepartment(string deviceId, string departmentName)
        {
            try
            {
                //按部门获取员工
                List<StaffModel> staffs = _staffDao.ListStaffByDepartment(departmentName);
                if (staffs.Count != 0)
                {
                    int rows = 0;
                    foreach (var s in staffs)
                    {
                        StaffAuthModel authModel = new StaffAuthModel
                        {
                            staffId = s.staffId,
                            deviceId = deviceId
                        };
                        _staffDao.AddAuth(authModel);
                        rows = rows + 1;
                    }
                    return rows;
                }
                else
                {
                    return "No staff In this Department";
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }    
        }
        /*********************/
        /*     员工统计      */
        /********************/

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
            List<StaffModel> staffs = _staffDao.ListStaffsByDeviceId(deviceId);
            List<StaffModel> inside = new List<StaffModel>();
            List<StaffModel> outside = new List<StaffModel>();
            List<StaffModel> absence = new List<StaffModel>();
            List<StaffModel> customers = new List<StaffModel>();
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //DateTime startTime = today;
            if(staffs.Count!=0)
            {
                foreach (var s in staffs)
                {
                    DeviceDataModel deviceData = _deviceDataDao.ListByDeviceIdAndIndexId(deviceId, s.staffId, today, today.AddDays(1), 1).FirstOrDefault();
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
            
        }
        /*获取昨日车间员工进出情况*/
        public object GetYesterdayStaffOnShop(string deviceId)
        {
            List<StaffModel> staffs = _staffDao.ListStaffsByDeviceId(deviceId);
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
                    
                    DeviceDataModel deviceData = _deviceDataDao.ListByDeviceIdAndIndexId(deviceId, s.staffId, startTime, startTime.AddDays(1), 2).FirstOrDefault();
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
                        if(s.department!="访客")
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
                    totalStaffs=inside.Union(outside),
                    absence = absence.Count(),
                    absenceStaffs = absence,
                    fixedStaff = staffs.Count()-customers.Count(),
                    customers = customers.Count(),
                    customerStaffs = customers,
                    description = 
                                "昨日打卡总人数：" + total + " 人;" +
                                "昨日缺勤人数：" + absence.Count() + " 人;"+
                                "固定人员："+(staffs.Count()-customers.Count())+" 人;"+
                                "外来人员："+customers.Count()+" 人;"
                };
            }
            else
            {
                throw new Exception("No staff in This Device");
                //return "No staff in This Device";
            }
        }
        /*获取员工考勤一段时间内的记录*/
        public object GetAttendenceRecord(string staffId,DateTime startTime,DateTime endTime)
        {
            List<string> devices = _staffDao.GetAuthDevice(staffId);
            Dictionary<string, List<DeviceDataModel>> records = new Dictionary<string, List<DeviceDataModel>>();
            foreach(var d in devices)
            {
                List<DeviceDataModel> datas = _deviceDataDao.ListByDeviceIdAndIndexId(d, staffId, startTime, endTime);
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
                List<DeviceDataModel> datas= _deviceDataDao.ListByDeviceIdAndValue(deviceId, 1, 10);
                List<object> result = new List<object>();
                if (datas.Count!=0)
                {
                    
                    foreach(var d in datas)
                    {
                        StaffModel staff = _staffDao.GetStaffInfoById(d.IndexId);
                        result.Add(new
                        {
                            staff=staff,
                            deviceId=d.DeviceId,
                            deviceName=d.DeviceName,
                            timeStamp=d.Timestamp,
                            
                        });
                    }
                }
                return result;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*获取最近离场的10人*/
        public object ListLatestExit10(string deviceId)
        {
            try
            {
                List<DeviceDataModel> datas = _deviceDataDao.ListByDeviceIdAndValue(deviceId, 0, 10);
                List<object> result = new List<object>();
                if (datas.Count != 0)
                {

                    foreach (var d in datas)
                    {
                        StaffModel staff = _staffDao.GetStaffInfoById(d.IndexId);
                        result.Add(new
                        {
                            staff=staff,
                            deviceId = d.DeviceId,
                            deviceName = d.DeviceName,
                            timeStamp = d.Timestamp
                        });
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        /*最新刷卡员工*/
        public object GetLatestOne(string deviceId)
        {
            try
            {
                DeviceDataModel latestData = _deviceDataDao.GetLastDataByDeviceId(deviceId);
                StaffModel staff = new StaffModel();
                if(latestData!=null)
                {
                    staff = _staffDao.GetStaffInfoById(latestData.IndexId);
                }
                return new
                {
                    staff = staff,
                    timeStamp = latestData.Timestamp,
                    status = latestData.IndexValue == 0 ? "离场" : "入场",
                    deviceId=deviceId
                };
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        




        /*模拟员工打卡使用*/
        /*查看员工当日最新的打卡状态 0表示当日打过卡，且已离开；1表示当前还在车间，-1表示今日未打卡*/
        public int GetCurrentStatusByStaffId(string deviceId, string staffId)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DeviceDataModel deviceData = _deviceDataDao.ListByDeviceIdAndIndexId(deviceId, staffId, today, today.AddDays(1), 1).FirstOrDefault();
            int status;
            if(deviceData!=null)
            {
                status = deviceData.IndexValue == 0 ? 0 : 1;
            }
            else
            {
                status = -1;
            }
            return status;
        }
        /*获取职称列表*/
        public object ListStaffRole()
        {
            try
            {
                return _staffDao.ListStaffRole();
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*添加职称*/
        public object AddStaffRole(string staffRole)
        {
            try
            {
                return _staffDao.AddStaffRole(staffRole);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*修改职称*/
        public object UpdateStaffRole(int id,string staffRole)
        {
            try
            {
                return _staffDao.UpdateStaffRole(id, staffRole);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        /*删除职称*/
        public object DeleteStaffRole(int  id)
        {
            try
            {
                return _staffDao.DeleteStaffRole(id);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

    }
}
