using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
using Dapper;

namespace IoTManager.Dao.MySQL
{
    public class MySQLStaffDao:IStaffDao
    {
        /* 
         * 通过工号获取员工信息
         */
        public StaffModel GetStaffInfoById(string staffId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
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
                    "staff.base64Image," +
                    "staff.pictureRoute, " +
                    "staffrole.staffRole " +
                    "FROM staff " +
                    "LEFT JOIN department ON department.id=staff.department "+
                    "LEFT JOIN staffrole ON staffrole.id=staff.staffRole "+
                    "WHERE staff.staffId=@sid ";
                return connection.Query<StaffModel>(sql, new { sid = staffId }).FirstOrDefault();
            }
        }
        /* 获取所有员工信息
         * 分页功能
         * 部分筛选功能
         * searchType="search"时，分页功能打开
         */
        public List<StaffModel> ListStaffs(string searchType,string department="all", int offset = 0, int limit = 12, string sortColumn = "id", string order = "asc")
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
                "staff.base64Image," +
                "staffrole.staffRole AS staffRole, " +
                "staff.pictureRoute " +
                "FROM staff " +
                "LEFT JOIN staffrole ON staffrole.id=staff.staffRole "+
                "LEFT JOIN department ON department.id=staff.department "+
                "WHERE staff.department=department.id ";
            if(searchType=="search")
            {
                if(department!="all")
                {
                    sql += " AND department.departmentName=@dn ";
                }
                if (order != "no" && sortColumn != "no")
                {
                    string orderBySubsentence = "order by " + sortColumn + " " + order;
                    sql += orderBySubsentence;
                }
                string limitSubsentence = " limit " + offset.ToString() + "," + limit.ToString();
                sql += limitSubsentence;
            }
           
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<StaffModel>(sql, new { dn=department })
                    .ToList();
            }
        }
        /*按照部门名称获取所有staff*/
        public List<StaffModel> ListStaffByDepartment(string departmentName)
        {
            string sql = "SELECT staff.id," +
                "staff.staffName," +
                "staff.staffId," +
                "staff.gender," +
                "staff.age," +
                "department.departmentName AS department," +
                "staffrole.staffRole AS staffRole, " +
                "staff.phoneNumber," +
                "staff.email," +
                "staff.remark," +
                "staff.createTime," +
                "staff.updateTime," +
                "staff.base64Image," +
                "staff.pictureRoute " +
                "FROM staff " +
                "LEFT JOIN staffrole ON staffrole.id=staff.staffRole "+
                "LEFT JOIN department ON department.id=staff.department "+
                "WHERE staff.department=department.id " +
                "AND department.departmentName=@dn";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<StaffModel>(sql, new { dn = departmentName })
                    .ToList();
            }
        }
        /*
         * 创建员工
         */
        public string Create(StaffModel staff)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                DepartmentModel department = connection.Query<DepartmentModel>(
                    "SELECT * FROM department WHERE department.departmentName=@dn", new { dn = staff.department }).FirstOrDefault();
                int staffrole = connection.Query<int>("SELECT id FROM staffrole WHERE staffRole=@sr", new { sr = staff.staffRole }).FirstOrDefault();
                int rows = connection.Execute("INSERT INTO staff(staffId,staffName,staffRole,gender,age,department,phoneNumber,email,remark) VALUES " +
                    "(@sid,@sr,@sn,@gen,@ag,@dep,@phn,@em,@re)", new
                    {
                        sid = staff.staffId,
                        sn = staff.staffName,
                        sr=staffrole,
                        gen = staff.gender,
                        ag = staff.age,
                        dep = department.id,
                        phn = staff.phoneNumber,
                        em = staff.email,
                        re = staff.remark
                    });
                return rows == 1 ? "success" : "error";
            }
        }
        /*修改员工信息*/
        public string Update(string staffId,StaffModel staff)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                DepartmentModel department = connection.Query<DepartmentModel>(
                    "SELECT * FROM department WHERE department.departmentName=@dn", new { dn = staff.department }).FirstOrDefault();
                int staffrole = connection.Query<int>("SELECT id FROM staffrole WHERE staffRole=@sr", new { sr = staff.staffRole }).FirstOrDefault();
                int rows = connection.Execute(
                    "UPDATE staff SET staffName=@sn," +
                    "staffRole=@sr,"+
                    "gender=@gen," +
                    "age=@ag," +
                    "department=@dep," +
                    "phoneNumber=@phn," +
                    "email=@em," +
                    "remark=@re," +
                    "updateTime=CURRENT_TIMESTAMP," +
                    "base64Image=@bi," +
                    "pictureRoute=@pr " +
                    "WHERE staffId=@sid", new
                    {
                        sn = staff.staffName,
                        sr=staffrole,
                        gen = staff.gender,
                        ag = staff.age,
                        dep = department.id,
                        phn = staff.phoneNumber,
                        em = staff.email,
                        re = staff.remark,
                        bi = staff.base64Image,
                        pr = staff.pictureRoute
                    });
                return rows == 1 ? "sucess" : "error";
            }
        }
        /*删除员工*/
        public string Delete(string staffId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("DELETE FROM staff WHERE staffId=@sid", new
                {
                    sid = staffId
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*批量删除员工*/
        public int BatchDelete(List<string> staffIds)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = 0;
                foreach (string i in staffIds)
                {
                    connection.Execute("DELETE FROM staff WHERE staffId=@sid", new
                    {
                        sid = i,
                    });
                    rows = rows + 1;
                }
                return rows;
            }
        }
        /*添加员工权限*/
        public string AddAuth(StaffAuthModel authModel)
        {
            string sql = "INSERT INTO staffauth(staff,device) VALUES " +
                "((SELECT id FROM staff WHERE staffId=@sid),(SELECT id FROM device WHERE hardwareDeviceID=@did))";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute(sql, new
                {
                    sid = authModel.staffId,
                    did=authModel.deviceId
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*通过staffId获取具有权限的门禁设备,返回deviceId列表*/
        public List<string> GetAuthDevice(string staffId)
        {
            string sql = "SELECT device.hardwareDeviceID FROM device WHERE device.id IN " +
                "(SELECT staffauth.device FROM staffauth WHERE staffauth.staff=(SELECT staff.id FROM staff WHERE staff.staffId=@sid))";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<string>(sql, new { sid = staffId })
                    .ToList();
            }
        }
        /*查找staffId是否被deviceId授权*/
        public bool Contain(string staffId,string deviceId)
        {
            string sql = "SELECT * FROM staffauth WHERE staffauth.staff=(SELECT staff.id FROM staff WHERE staff.staffId=@sid) " +
                "AND staffauth.device=(SELECT device.id FROM device WHERE device.hardwareDeviceID=@did)";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute(sql, new
                {
                    sid = staffId,
                    did = deviceId
                });
                return rows == 1 ? true : false;
            }
        }
        /*删除员工staffId的权限*/
        public string DeleteAuth(string staffId,string deviceId)
        {
            string sql = "DELETE FROM staffauth WHERE staffauth.staff=(SELECT staff.id FROM staff WHERE staff.staffId=@sid) " +
                "AND staffauth.device=(SELECT device.id FROM device WHERE device.hardwareDeviceID=@did)";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute(sql, new
                {
                    sid = staffId,
                    did = deviceId
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*批量删除*/
        public int BatchDeleteAuth(string staffId,List<string> deviceIds)
        {
            string sql = "DELETE FROM staffauth WHERE staffauth.staff=(SELECT staff.id FROM staff WHERE staff.staffId=@sid) " +
                "AND staffauth.device=(SELECT device.id FROM device WHERE device.hardwareDeviceID=@did)";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = 0;
                foreach (string i in deviceIds)
                {
                    connection.Execute(sql, new
                    {
                        sid=staffId,
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
         * 输出设备的下的所有授权员工工号（属性Id）
         *
         */
        public List<string> ListStaffIdsByDeviceId(string deviceId)
        {
            string sql = "SELECT staff.staffId FROM staff WHERE staff.id IN " +
                "(SELECT staffauth.staff FROM staffauth WHERE staffauth.device IN " +
                "(SELECT device.id FROM device WHERE device.hardwareDeviceID=@hid))";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<string>(sql, new { hid = deviceId })
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
                "staff.base64Image," +
                "staff.pictureRoute, " +
                "staffrole.staffRole AS staffRole "+
                "FROM staff " +
                "LEFT JOIN department ON department.id=staff.department "+
                "LEFT JOIN staffrole ON staffrole.id=staff.staffRole "+
                "WHERE staff.id IN " +
                "(SELECT staffauth.staff FROM staffauth WHERE staffauth.device IN " +
                "(SELECT device.id FROM device WHERE device.hardwareDeviceID=@hid))";
                
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<StaffModel>(sql, new { hid = deviceId })
                    .ToList();
            }
        }


        /*员工职称*/
        /*获取员工职称列表*/
        public List<object> ListStaffRole()
        {
            string sql = "SELECT id,staffRole FROM staffrole";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<object>(sql)
                    .ToList();
            }
        }
        /*添加职称*/
        public string AddStaffRole(string staffRole)
        {
            string sql = "INSERT INTO staffrole(staffRole,createTime) VALUES (@sr,CURRENT_TIMESTAMP)";
            string s = "SELECT * FROM staffrole WHERE staffRole=@sr";

            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute(s, new { sr = staffRole });
                if(rows==0)
                {
                    return connection.Execute(sql, new { sr = staffRole }) == 1 ? "success" : "error";
                }
                else
                {
                    return "exist";
                }
            }
        }
        /*修改职称*/
        public string UpdateStaffRole(int id,string staffRole)
        {
            string sql = "UPDATE staffrole SET staffRole=@sr,updateTime=CURRENT_TIMESTAMP WHERE id=@id";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Execute(sql, new { sr = staffRole,id=id }) == 1 ? "success" : "error";
            }
        }
        /*删除职称*/
        public string DeleteStaffRole(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("DELETE FROM staffrole WHERE id=@id", new
                {
                    id = id
                });
                return rows == 1 ? "success" : "error";
            }
        }

    }
}
