using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLSeverityDao : ISeverityDao
    {
        public List<SeverityModel> Get()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<SeverityModel>("select * from severity").ToList();
            }
        }

        public SeverityModel GetById(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<SeverityModel>("select * from severity where id=@sid", new { sid = id })
                    .FirstOrDefault();
            }
        }
    }
}