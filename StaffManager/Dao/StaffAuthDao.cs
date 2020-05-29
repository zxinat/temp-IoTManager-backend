using Dapper;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
using StaffManager.Dao.Infrastructures;
using StaffManager.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using System;

namespace StaffManager.Dao
{
    public class StaffAuthDao : IStaffAuthDao
    {
        private readonly DatabaseConStr _databaseConStr;
        private readonly string connectionStr;
        public StaffAuthDao(IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
            connectionStr = _databaseConStr.MySQL;
        }
        /*添加员工门禁权限*/
        public string AddAuth(StaffAuthModel authModel)
        {
            string sql = "INSERT INTO staffauth(staffId,device) VALUES " +
                "(@sid,(SELECT id FROM device WHERE hardwareDeviceID=@did))";
            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = connection.Execute(sql, new
                {
                    sid = authModel.staffId,
                    did = authModel.deviceId
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*通过staffId获取具有权限的门禁设备,返回deviceId列表*/
        public List<string> GetAuthDevice(string staffId)
        {
            string sql = "SELECT device.hardwareDeviceID FROM device WHERE device.id IN " +
                "(SELECT staffauth.device FROM staffauth WHERE staffauth.staffId=@sid)";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<string>(sql, new { sid = staffId })
                    .ToList();
            }
        }
        /*查找staffId是否被deviceId授权*/
        public bool Contain(string staffId, string deviceId)
        {
            string sql = "SELECT COUNT(*) FROM staffauth WHERE staffauth.staffId=@sid " +
                "AND staffauth.device=(SELECT device.id FROM device WHERE device.hardwareDeviceID=@did)";
            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = connection.Query<int>(sql, new
                {
                    sid = staffId,
                    did = deviceId
                }).FirstOrDefault();
                return rows == 0 ? false : true;
            }
        }
        /*删除员工staffId的权限*/
        public string DeleteAuth(string staffId, string deviceId)
        {
            string sql = "DELETE FROM staffauth WHERE staffauth.staffId=@sid " +
                "AND staffauth.device=(SELECT device.id FROM device WHERE device.hardwareDeviceID=@did)";
            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = connection.Execute(sql, new
                {
                    sid = staffId,
                    did = deviceId
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*删除staffId的所有权限*/
        public string DeleteAuthByStaffId(string staffId)
        {
            string sql = "DELETE FROM staffauth WHERE staffauth.staffId=@sid";
            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = connection.Execute(sql, new
                {
                    sid = staffId
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*批量删除*/
        public int BatchDeleteAuth(string staffId, List<string> deviceIds)
        {
            string sql = "DELETE FROM staffauth WHERE staffauth.staffId=@sid " +
                "AND staffauth.device=(SELECT device.id FROM device WHERE device.hardwareDeviceID=@did)";
            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = 0;
                foreach (string i in deviceIds)
                {
                    connection.Execute(sql, new
                    {
                        sid = staffId,
                        did = i
                    });
                    rows = rows + 1;
                }
                return rows;
            }
        }
        /* 通过设备查找、添加、删除授权员工
         *
         */

        /* 输入设备Id
         * 输出设备的下的所有授权正式员工工号（属性Id）
         *
         */
        public List<string> ListStaffIdsByDeviceId(string deviceId)
        {
            string sql = "SELECT staffauth.staffId FROM staffauth WHERE staffauth.staffId IN " +
                "(SELECT rfidtag.staffId FROM rfidtag WHERE rfidtag.type=1) AND staffauth.device IN " +
                "(SELECT id FROM device WHERE device.hardwareDeviceID=@hid)";
            //string sql = "SELECT staffauth.staffId FROM staffauth WHERE staffauth.device IN " +
            //    "(SELECT device.id FROM device WHERE device.hardwareDeviceID=@hid)";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<string>(sql, new { hid = deviceId })
                    .ToList();
            }
        }
        /*获取设备DeviceId下授权的员工*/
        public List<string> ListStaffIdsByDeviceId(string deviceId,DateTime startTime,DateTime endTime)
        {
            string date = endTime.ToLocalTime().ToString(Constant.getMysqlDateFormString());
            string sql = "SELECT staffauth.staffId FROM staffauth WHERE staffauth.staffId IN " +
                "(SELECT rfidtag.staffId FROM rfidtag WHERE rfidtag.type=1) AND staffauth.device IN " +
                "(SELECT id FROM device WHERE device.hardwareDeviceID=@hid) AND staffauth.timestamp<=@ts";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<string>(sql, new { hid = deviceId,ts=date })
                    .ToList();
            }
        }
        /*按设备Id获取员工*/
        public List<StaffModel> ListStaffsByDeviceId(string deviceId)
        {
            string sql = "SELECT staff.id," +
                "staff.staffName," +
                "staff.staffId," +
                "staff.gender," +
                "staff.age," +
                "department.departmentName AS department," +
                "staff.phoneNumber," +
                "staff.email," +
                "staff.remark," +
                "staff.createTime," +
                "staff.updateTime," +
                "staff.status, " +
                "staff.lastTime, " +
                "staffrole.staffRole AS staffRole " +
                "FROM staff " +
                "LEFT JOIN department ON department.id=staff.department " +
                "LEFT JOIN staffrole ON staffrole.id=staff.staffRole " +
                "WHERE staff.staffId IN " +
                "(SELECT staffauth.staffId FROM staffauth WHERE staffauth.device IN " +
                "(SELECT device.id FROM device WHERE device.hardwareDeviceID=@hid))";

            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<StaffModel>(sql, new { hid = deviceId })
                    .ToList();
            }
        }
        /*通过deviceId获取时间段捏授权的访客*/
        public List<CustomerModel> ListCustomerBydeviceId(string deviceId)
        {
            return null;
        }

    }
}
