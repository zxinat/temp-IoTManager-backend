using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Dao
{
    public sealed class MySQLDepartmentDao : IDepartmentDao
    {
        /*获取所有部门*/
        public List<DepartmentModel> List()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                string sql = "SELECT * FROM department";
                return connection.Query<DepartmentModel>(sql).ToList();
            }
        }
        /*通过名称获取部门*/
        public DepartmentModel GetByName(string departmentName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                string sql = "SELECT * FROM department WHERE departmentName=@dn";
                return connection.Query<DepartmentModel>(sql, new { dn = departmentName }).FirstOrDefault();
            }
        }
        /*通过id获取部门信息*/
        public DepartmentModel GetById(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                string sql = "SELECT * FROM department WHERE id=@id";
                return connection.Query<DepartmentModel>(sql, new { id=id }).FirstOrDefault();
            }
        }
        /*创建部门*/
        public string Create(DepartmentModel department)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                string sql = "INSERT INTO department(departmentName,admin,remark) VALUES " +
                    "(@dn,@ad,@re)";
                int rows = connection.Execute(sql, new { dn = department.departmentName, ad = department.admin, re = department.remark });
                return rows == 1 ? "success" : "error";
            }
        }
        /*删除部门*/
        public string Delete(string departmentName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("DELETE FROM department WHERE departmentName=@dn", new
                {
                    dn = departmentName
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*修改部门信息*/
        public string Update(int id,DepartmentModel department)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute(
                    "UPDATE department SET departmentName=@dn," +
                    "admin=@ad," +
                    "remark=@re " +
                    "WHERE id=@id", new
                    {
                        dn = department.departmentName,
                        ad = department.admin,
                        re = department.remark,
                        id = id
                    });
                return rows == 1 ? "sucess" : "error";
            }
        }
        /*获取部门内员工列表*/
        public List<StaffModel> ListStaffsBydepartment(string department)
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
                "staffrole.staffRole as staffRole " +
                "FROM staff LEFT JOIN department ON department.id=staff.department " +
                "LEFT JOIN staffrole ON staffrole.id=staff.staffRole " +
                "WHERE staff.department IN " +
                "(SELECT department.id FROM department WHERE department.departmentName=@dn)";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<StaffModel>(sql, new { dn = department })
                    .ToList();
            }
        }

        /*获取部门内的员工总数*/
        public int GetTotalByDepartment(string department)
        {
            string sql = "SELECT COUNT(*) FROM staff " +
                "WHERE staff.department IN (SELECT department.id FROM department WHERE department.departmentName=@dn)";
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int result = connection.Query<int>(sql, new { dn = department }).FirstOrDefault();
                return result;
            }
        }
    }
}
