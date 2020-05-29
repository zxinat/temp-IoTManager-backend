using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StaffManager.Core.Infrastructures;
using StaffManager.Dao.Infrastructures;
using StaffManager.Models;

namespace StaffManager.Core
{
    public class StaffAuthBus:IStaffAuthBus
    {
        private readonly IStaffDao _staffDao;
        private readonly ILogger _logger;
        private readonly IStaffAuthDao _staffAuthDao;

        public StaffAuthBus(IStaffDao staffDao,
            ILogger<StaffAuthBus> logger,
            IStaffAuthDao staffAuthDao)
        {
            _staffDao = staffDao;
            _logger = logger;
            _staffAuthDao = staffAuthDao;
        }
        /********************/
        /*   职工权限管理    */
        /********************/
        /*添加员工权限*/
        public string AddAuth(StaffAuthModel staffAuth)
        {

            if (!_staffAuthDao.Contain(staffAuth.staffId, staffAuth.deviceId))
            {
                return _staffAuthDao.AddAuth(staffAuth);
            }
            else
            {
                return "exist";
            }
        }
        /*批量给员工添加权限*/
        public object BatchAddAuth(string staffId, List<string> deviceIds)
        {
            int rows = 0;
            foreach (var did in deviceIds)
            {
                if (!_staffAuthDao.Contain(staffId, did))
                {
                    StaffAuthModel staffAuth = new StaffAuthModel
                    {
                        staffId = staffId,
                        deviceId = did
                    };
                    _staffAuthDao.AddAuth(staffAuth);
                    rows = rows + 1;
                }
            }
            return rows;

        }
        /*列出授权的设备*/
        public object GetAuthDevice(string staffId)
        {

            return _staffAuthDao.GetAuthDevice(staffId);
        }
        /*删除员工的权限*/
        public string DeleteAuth(string staffId, string deviceId)
        {
            return _staffAuthDao.DeleteAuth(staffId, deviceId);
        }
        /*批量删除员工权限*/
        public object BatchDeleteAuth(string staffId, List<string> deviceIds)
        {
            return _staffAuthDao.BatchDeleteAuth(staffId, deviceIds);

        }

        /*获取设备授权的工号*/
        public object ListStaffIdsByDeviceId(string deviceId)
        {

            return _staffAuthDao.ListStaffIdsByDeviceId(deviceId);
        }
        /*按部门分配门禁权限*/
        public int AddAuthByDepartment(string deviceId, string departmentName)
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
                    _staffAuthDao.AddAuth(authModel);
                    rows = rows + 1;
                }
                return rows;
            }
            else
            {
                return -1;
            }
        }
    }
}
