using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
using StaffManager.Dao.Infrastructures;
using Dapper;
using StaffManager.Models;
using Microsoft.Extensions.Options;

namespace StaffManager.Dao
{
    public class StaffDao:IStaffDao
    {
        private readonly string connectionStr;
        private readonly DatabaseConStr _databaseConStr;
        //private readonly MySqlConnection connection;
        public StaffDao(IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
            connectionStr = connectionStr = _databaseConStr.MySQL;
            //connection = new MySqlConnection(connectionStr);
        }
        /* 
         * 通过工号获取员工信息,照片信息另外获取
         */
        public StaffModel GetStaffInfoById(string staffId)
        {
            using (var connection = new MySqlConnection(connectionStr))
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
                    "staffrole.staffRole, " +
                    "staff.status, "+
                    "staff.lastTime "+
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
                "staff.status, "+
                "staff.lastTime "+
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
           
            using (var connection = new MySqlConnection(connectionStr))
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
                "staff.status, " +
                "staff.lastTime "+
                "FROM staff " +
                "LEFT JOIN staffrole ON staffrole.id=staff.staffRole "+
                "LEFT JOIN department ON department.id=staff.department "+
                "WHERE staff.department=department.id " +
                "AND department.departmentName=@dn";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<StaffModel>(sql, new { dn = departmentName })
                    .ToList();
            }
        }
        /* 按时间获取部门内所有staffId
         * 输入：时间段，部门名称；输出list
         */
        public List<string> ListStaffIdsByDepartmentAndDate(string departmentName,DateTime startTime,DateTime endTime)
        {
            string sql = "SELECT staff.staffId FROM staff JOIN department ON department.id=staff.department " +
                "AND department.departmentName=@dn AND staff.createTime<=@ct";
            using (var connection = new MySqlConnection(connectionStr))
            {
                string date = endTime.ToLocalTime().ToString(Constant.getMysqlDateFormString());
                return connection.Query<string>(sql, new { dn = departmentName,ct= date})
                    .ToList();
            }
        }

        /*
         * 创建员工
         */
        public string Create(StaffModel staff)
        {
            using (var connection = new MySqlConnection(connectionStr))
            {
                DepartmentModel department = connection.Query<DepartmentModel>(
                    "SELECT * FROM department WHERE department.departmentName=@dn", new { dn = staff.department }).FirstOrDefault();
                int staffrole = connection.Query<int>("SELECT id FROM staffrole WHERE staffRole=@sr", new { sr = staff.staffRole }).FirstOrDefault();
                int rows = connection.Execute("INSERT INTO staff(staffId,staffName,staffRole,gender,age,department,phoneNumber,email,remark,status) VALUES " +
                    "(@sid,@sn,@sr,@gen,@ag,@dep,@phn,@em,@re,@sta)", new
                    {
                        sid = staff.staffId,
                        sn = staff.staffName,
                        sr=staffrole,
                        gen = staff.gender,
                        ag = staff.age,
                        dep = department.id,
                        phn = staff.phoneNumber,
                        em = staff.email,
                        re = staff.remark,
                        sta=staff.status
                    });
                return rows == 1 ? "success" : "error";
            }
        }
        /*修改员工信息*/
        public string Update(string staffId,StaffModel staff)
        {
            using (var connection = new MySqlConnection(connectionStr))
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
                    "pictureRoute=@pr, " +
                    "status=@sta, "+
                    "lastTime=@la "+
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
                        pr = staff.pictureRoute,
                        sid=staffId,
                        sta=staff.status,
                        la=staff.lastTime
                    });
                return rows == 1 ? "success" : "error";
            }
        }
        /*删除员工*/
        public string Delete(string staffId)
        {
            using (var connection = new MySqlConnection(connectionStr))
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
            using (var connection = new MySqlConnection(connectionStr))
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
        
        





        
        /*员工职称*/
        /*获取员工职称列表*/
        public List<object> ListStaffRole()
        {
            string sql = "SELECT id,staffRole FROM staffrole";
            using (var connection = new MySqlConnection(connectionStr))
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

            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = connection.Execute(s, new { sr = staffRole });
                if(rows==-1)
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
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Execute(sql, new { sr = staffRole,id=id }) == 1 ? "success" : "error";
            }
        }
        /*删除职称*/
        public string DeleteStaffRole(int id)
        {
            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = connection.Execute("DELETE FROM staffrole WHERE id=@id", new
                {
                    id = id
                });
                return rows == 1 ? "success" : "error";
            }
        }

        /*获取staff中所有staffId*/
        public List<string> ListStaffIds(string search="staff")
        {
            string sql = search == "staff" ? "SELECT staffId FROM staff WHERE status=1" :
                "SELECT staffId FROM staff";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<string>(sql).ToList();
            }
        }

    }
}
