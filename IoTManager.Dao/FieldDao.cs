using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;

namespace IoTManager.Dao
{
    public sealed class FieldDao: IFieldDao
    {
        public List<FieldModel> Get()
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                List<FieldModel> fields = connection.Query<FieldModel>(
                    "select * from field ").ToList();
                return fields;
            }
        }
    }
}