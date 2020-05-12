using IoTManager.IDao;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
using Dapper;

namespace IoTManager.Dao.MySQL
{
    public class MySQLCustomerDao:ICustomerDao
    {
        /* 获取所有外来人员信息
         * 分页功能
         * 部分筛选功能
         * searchType="search"时，分页功能打开
         */
         public List<CustomerModel> ListCustomers(string searchType, string department = "all", int offset = 0, int limit = 12, string sortColumn = "id", string order = "asc")
        {
            string sql = "";
            return null;
        }

        /*判断staffId是否存在，staff表和customer表均不能有重复staffId*/
        /*列出已注册的staffId*/
        public List<string> ListAllStaffIds()
        {
            return null;
        }
        /*添加外来人员*/
        public string Create(CustomerModel customer)
        {
            return null;
        }
    }
}
