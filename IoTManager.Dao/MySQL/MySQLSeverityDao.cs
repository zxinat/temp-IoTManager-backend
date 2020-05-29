using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    
    public sealed class MySQLSeverityDao : ISeverityDao
    {
        private readonly DatabaseConStr _databaseConStr;
        public MySQLSeverityDao(IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
        }
        public List<SeverityModel> Get()
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection.Query<SeverityModel>("select * from severity").ToList();
            }
        }

        public SeverityModel GetById(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection.Query<SeverityModel>("select * from severity where id=@sid", new { sid = id })
                    .FirstOrDefault();
            }
        }
    }
}