using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IoTManager.IDao;
using StaffManager.Core.Infrastructures;
using StaffManager.Dao.Infrastructures;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using StaffManager.Models.RequestModel;
using StaffManager.Models;
using StaffManager.Models.Serializers;
using StaffManager.Models.StatisticModels;

namespace StaffManager.Core
{
    public class StaffBus:IStaffBus
    {
        private readonly IStaffDao _staffDao;
        private readonly ILogger _logger;
        private readonly IStaffDataDao _staffData;
        private readonly IStaffAuthDao _staffAuthDao;
        private readonly IDeviceDao _deviceDao;
        private readonly IRFIDTagBus _rFIDTagBus;
        private readonly IRFIDTagDao _rFIDTagDao;
        private readonly ICustomerDao _customerDao;
        /*构造函数注入*/
        public StaffBus(IStaffDao staffDao,
            ILogger<StaffBus> logger,
            IStaffDataDao staffData, 
            IDeviceDao deviceDao,
            IStaffAuthDao staffAuthDao,
            IRFIDTagBus rFIDTagBus,
            IRFIDTagDao rFIDTagDao,
            ICustomerDao customerDao)
        {
            _staffDao = staffDao;
            _logger = logger;
            _staffData = staffData;
            _deviceDao = deviceDao;
            _staffAuthDao = staffAuthDao;
            _rFIDTagBus = rFIDTagBus;
            _rFIDTagDao = rFIDTagDao;
            _customerDao = customerDao;
        }
        /********************/
        /*    职工信息管理   */
        /********************/
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

            StaffModel staff= _staffDao.GetStaffInfoById(staffId);
            return new StaffSerializer(staff);

        }
        /*创建员工*/
        public string Create(StaffFormModel staffForm)
        {
            StaffModel staff = new StaffModel(staffForm);
            try
            {
                StaffModel eStaff = _staffDao.GetStaffInfoById(staffForm.staffId);
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
        public string Update(string staffId, StaffFormModel staffForm)
        {

            StaffModel staff=_staffDao.GetStaffInfoById(staffId);
            staff.staffName = staffForm.staffName;
            staff.staffRole = staffForm.staffRole;
            staff.gender = staffForm.gender;
            staff.age = staffForm.age;
            staff.department = staffForm.department;
            staff.phoneNumber = staffForm.phoneNumber;
            staff.email = staffForm.email;
            staff.remark = staffForm.remark;
            if(staff.lastTime<=new DateTime(1970,1,1))
            {
                staff.lastTime = new DateTime(1970, 1, 1);
            }
            string nowStatus= staff.status==1 ? "在职" : "离职";
            if(nowStatus == "在职" & staffForm.status == "离职")
            {
                staff.status = 0;
                staff.lastTime = DateTime.Now;
            }
            else if(nowStatus == "离职" & staffForm.status == "在职")
            {
                staff.status = 1;
            }
            return _staffDao.Update(staffId, staff);
        }
        /*员工离职*/
        public string Logout(string staffId,string status)
        {
            StaffModel staff = _staffDao.GetStaffInfoById(staffId);
            string nowStatus = staff.status == 1 ? "在职" : "离职";
            string result;
            if(nowStatus=="在职"&status=="离职")
            {
                staff.status = 0;
                staff.lastTime = DateTime.Now;
                result= _staffDao.Update(staffId, staff);
            }
            else if(nowStatus=="离职"&status=="在职")
            {
                staff.status = 1;
                result= _staffDao.Update(staffId, staff);
            }
            else
            {
                result = "success";
            }
            return result;
        }
        /*上传照片*/
        public string UpLoadImage(string staffId,string base64Image)
        {

            StaffModel staff = _staffDao.GetStaffInfoById(staffId);
            staff.base64Image = base64Image;
            return _staffDao.Update(staffId, staff);
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

        /**********/
        /*添加数据*/
        public void CreateData(StaffDataFormModel sd)
        {
            StaffDataModel staffData = new StaffDataModel
            {
                DeviceId = sd.DeviceId,
                DeviceName = sd.DeviceName,
                IndexId = sd.MonitorId,
                IndexName = sd.MonitorName,
                IndexType = sd.MonitorType,
                IndexUnit = sd.Unit,
                IndexValue = sd.value,
                Timestamp = sd.Timestamp,
                Inspected = sd.IsScam,
                GatewayId = sd.GatewayId,
                Mark = sd.Mark,
                IsCheck = sd.IsCheck,
                DeviceType = sd.DeviceType
            };
            _staffData.Create(staffData);

        }
        /*批量添加数据*/
        public int BatchCreateData(List<StaffDataFormModel> staffDataForms)
        {
            int rows = 0;
            foreach(var sd in staffDataForms)
            {
                CreateData(sd);
                rows++;
            }
            return rows;
        }
    }
}
