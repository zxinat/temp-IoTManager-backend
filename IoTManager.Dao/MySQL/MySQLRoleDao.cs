using IoTManager.IDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using IoTManager.Utility;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLRoleDao:IRoleDao
    {
        public String DeleteAllAuth(String roleId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var role = connection.Query(String.Format("select id from role where roleName=\"{0}\"",roleId)).ToList()[0];
                int count = connection.Query<int>("select count(*) FROM roleauth WHERE role=@rId", new
                {
                    rId = role.id
                }).FirstOrDefault();
                int rows = connection.Execute("DELETE FROM roleauth WHERE role=@rId", new
                {
                    rId = role.id
                });
                return rows == count ? "success" : "error";
            }
        }

        public String InsertAllAuth(String roleId, List<String> authId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var role = connection.Query(String.Format("select id from role where roleName=\"{0}\"",roleId)).ToList()[0];
                String condition = "";
                for(int i = 0; i < authId.Count; i++)
                {
                    if (i != authId.Count - 1)
                    {
                        condition += String.Format("authId=\"{0}\" or ", authId[i]);
                    }
                    else
                    {
                        condition += String.Format("authId=\"{0}\"", authId[i]);
                    }
                }
                Console.WriteLine("select id from auth where " + condition);
                var auth = connection.Query("select id from auth where " + condition).ToList();
                String items = "";
                for (int i = 0; i < auth.Count; i++)
                {
                    if (i != auth.Count - 1)
                    {
                        items += String.Format("({0}, {1}), ", role.id, auth[i].id);
                    }
                    else
                    {
                        items += String.Format("({0}, {1})", role.id, auth[i].id);
                    }
                }
                int rows = connection.Execute("insert into roleauth (role, auth) values " + items);
                return rows == auth.Count ? "success" : "error";
            }
        }
    }
}
