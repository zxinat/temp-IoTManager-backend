using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;

namespace IoTManager.Dao
{
    public sealed class SeverityDao: ISeverityDao
    {
        public List<SeverityModel> Get()
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<SeverityModel>("select * from severity").ToList();
            }
        }

        public SeverityModel GetById(int id)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<SeverityModel>("select * from severity where id=@sid", new {sid = id})
                    .FirstOrDefault();
            }
        }
    }
}