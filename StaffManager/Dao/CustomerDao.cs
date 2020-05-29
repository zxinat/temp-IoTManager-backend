using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using StaffManager.Dao.Infrastructures;
using IoTManager.Model;
using IoTManager.Utility;
using StaffManager.Models;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.Extensions.Options;

namespace StaffManager.Dao
{
    public class CustomerDao : ICustomerDao
    {
        private readonly DatabaseConStr _databaseConStr;
        private string mysqlConnectionStr;
        public CustomerDao(IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
            this.mysqlConnectionStr = _databaseConStr.MySQL;
        }
        /* 获取所有外来人员信息
         * 分页功能
         * 部分筛选功能
         * searchType="search"时，分页功能打开
         */
        public List<CustomerModel> ListCustomers(string searchType, int offset = 0, int limit = 12, string sortColumn = "id", string order = "asc")
        {
            string sql = "SELECT * FROM `customer` ";
            if (searchType == "search")
            {
                if (order != "no" && sortColumn != "no")
                {
                    string orderBySubsentence = "order by " + sortColumn + " " + order;
                    sql += orderBySubsentence;
                }
                string limitSubsentence = " limit " + offset.ToString() + "," + limit.ToString();
                sql += limitSubsentence;
            }
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                return connection.Query<CustomerModel>(sql).ToList();
            }
        }

        /*判断staffId是否存在，staff表和customer表均不能有重复staffId*/
        /*列出已注册的staffId*/
        public List<string> ListAllStaffIds()
        {
            //从staff表中获取
            string sql1 = "SELECT staffId FROM staff";
            //从customer中获取
            string sql2 = "SELECT staffId FROM customer WHERE status=1";
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                var re1 = connection.Query<string>(sql1).ToList();
                var re2 = connection.Query<string>(sql2).ToList();
                return re1.Union(re2).ToList();
            }
        }
        /*列出访客staffId列表*/
        public List<string> ListCustomerStaffIds()
        {
            return null;
        }
        /*添加外来人员*/
        public string Create(CustomerModel customer)
        {
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                int rows = connection.Execute("INSERT INTO customer(staffId,name,gender,phoneNumber,affiliation,cause,remark,status,lastArea) " +
                    "VALUES (@sid,@na,@gen,@ph,@afi,@ca,@re,@sta,@la)", new
                    {
                        sid = customer.staffId,
                        na = customer.name,
                        gen = customer.gender,
                        ph = customer.phoneNumber,
                        afi = customer.affiliation,
                        ca = customer.cause,
                        re = customer.remark,
                        sta = customer.status,
                        la = customer.lastArea
                    });
                return rows == 1 ? "success" : "error";

            }
        }
        /*修改外来人员信息*/
        public string Update(int id, CustomerModel customer)
        {
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                int rows = connection.Execute(
                    "UPDATE customer SET staffId=@sid," +
                    "name=@na," +
                    "gender=@gen," +
                    "phoneNumber=@ph," +
                    "affiliation=@afi," +
                    "cause=@ca," +
                    "remark=@re," +
                    "status=@sta," +
                    "lastArea=@la," +
                    "lastTime=@lt", new
                    {
                        sid = customer.staffId,
                        na = customer.name,
                        gen = customer.gender,
                        ph = customer.phoneNumber,
                        afi = customer.affiliation,
                        ca = customer.cause,
                        re = customer.remark,
                        sta = customer.status,
                        la = customer.lastArea,
                        lt = customer.lastTime
                    });
                return rows == 1 ? "success" : "error";
            }
        }
        /*获取持卡访客信息*/
        public CustomerModel GetCurrentByStaffId(string staffId)
        {
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                //获取当前门禁卡持有者的信息
                string sql = "SELECT * FROM customer WHERE customer.staffId=@sid AND status=1";
                return connection.Query<CustomerModel>(sql, new { sid = staffId }).FirstOrDefault();
            }
        }
        /*通过staffId获取一段时间内持有相同staffId的访客列表*/
        public List<CustomerModel> ListBystaffIdAndDate(string staffId,DateTime startTime,DateTime endTime)
        {
            string date = endTime.ToLocalTime().ToString(Constant.getMysqlDateFormString());
            string sql = "SELECT * FROM customer WHERE staffId=@sid AND createTime<=@ct";
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                return connection.Query<CustomerModel>(sql, new { sid = staffId, ct = date }).ToList();
            }
        }
        /*删除访客*/
        public string Delete(int id)
        {
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                if(IsExist(id))
                {
                    int rows = connection.Execute("DELETE FROM customer WHERE id=@id", new { id = id });
                    return rows == 1 ? "sucess" : "error";
                }
                else
                {
                    return "NoContent";
                }
                
            }
        }
        /*判断Id是否存在*/
        public bool IsExist(int id)
        {
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                int rows = connection.Execute("SELECT * FROM customer WHERE id=@id", new { id = id });
                return rows == 1 ? true : false;
            }
        }
        /*通过Id获取访客*/
        public CustomerModel GetById(int id)
        {
            string sql = "SELECT * FROM customer WHERE id=@id";
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                return connection.Query<CustomerModel>(sql,new { id = id }).FirstOrDefault();
            }
        }
        /*查看访客权限*/
        public List<string> ListAuth(int id)
        {
            string sql = "SELECT device.deviceName AS device FROM customerauth " +
                "LEFT JOIN device ON customerauth.device=device.id WHERE customer=@id";
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                return connection.Query<string>(sql, new { id = id }).ToList();
            }
        }

        /*
         *  访客权限配置
         */
        public string AddAuth(CustomerModel customer,string deviceId)
        {
            string sql = "INSERT INTO staffauth(staffId,device) VALUES " +
                "(@sid,(SELECT id FROM device WHERE hardwareDeviceID=@did))";
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                int rows = connection.Execute(sql, new
                {
                    sid = customer.staffId, 
                    did = deviceId
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*批量添加权限*/
        public int BatchAddAuth(CustomerModel customer,List<string> deviceIds)
        {
            string s = "SELECT id FROM device WHERE hardwareDeviceID=@did";
            string sql = "DELETE FROM customerauth WHERE customer=@cu AND device=@d";
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                int rows = 0;
                foreach(string i in deviceIds)
                {
                    int d = connection.Query<int>(s, new { did = i }).FirstOrDefault();
                    connection.Execute(sql, new
                    {
                        cu = customer.id,
                        d = d
                    });
                    rows = rows + 1;
                }
                return rows;
            }
        }
        public string DeleteAuth(CustomerModel customer,string deviceId)
        {
            string s = "SELECT id FROM device WHERE hardwareDeviceID=@did";
            string sql = "DELETE FROM customerauth WHERE customer=@cu AND device=@d";
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                int d = connection.Query<int>(s, new { did = deviceId }).FirstOrDefault();
                int rows = connection.Execute(sql, new
                {
                    cu = customer.id,
                    d = d
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*批量删除权限*/
        public int BatchDeleteAuth(CustomerModel customer,List<string> deviceIds)
        {
            string s = "SELECT id FROM device WHERE hardwareDeviceID=@did";
            string sql = "DELETE FROM customerauth WHERE customer=@cu AND device=@d";
            using (var connection = new MySqlConnection(mysqlConnectionStr))
            {
                int rows = 0;
                foreach(string i in deviceIds)
                {
                    int d = connection.Query<int>(s, new { did = i }).FirstOrDefault();
                    connection.Execute(sql, new
                    {
                        cu = customer.id,
                        d = d
                    });
                    rows = rows + 1;
                }
                return rows;
            }
        }
    }
}
