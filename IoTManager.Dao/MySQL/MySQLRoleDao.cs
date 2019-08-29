using IoTManager.IDao;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public String UpdateUserAuth(int userId, Dictionary<String, int> dic)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var user = connection.Query("select identify from account where id=@uId", new
                {
                    uid = userId
                }).ToList()[0];
                int count = connection.Query<int>("select count(*) FROM accountauth WHERE account=@uId", new
                {
                    uId = userId
                }).FirstOrDefault();
                int rows = connection.Execute("DELETE FROM accountauth WHERE account=@uId", new
                {
                    uId = userId
                });
                if (rows != count) return "error";
                if (dic.Count == 0)
                {
                    int rows2 = connection.Execute("update account set identify=0 where id=@uId", new
                    {
                        uId = userId
                    });
                    return rows2 == 1 ? "success" : "error";
                }
                else
                {
                    int rows2 = connection.Execute("update account set identify=1 where id=@uId", new
                    {
                        uId = userId
                    });
                    if (rows2 != 1) return "error";
                    String items = "";
                    foreach (var d in dic)
                    {
                        var auth = connection.Query(String.Format("select id from auth where authId=\"{0}\"", d.Key))
                            .ToList()[0];
                        if (d.Value == 1)
                        {
                            items += String.Format("(\"ADD\", {0}, {1}), ", userId, auth.id);
                        }
                        else
                        {
                            items += String.Format("(\"DELETE\", {0}, {1}), ", userId, auth.id);
                        }
                    }

                    items = items.Substring(0, items.Length - 2);
                    int rows3 = connection.Execute("insert into accountauth (operation, account, auth) values " + items);
                    return rows3 == dic.Count ? "success" : "error";
                }

            }
        }

        public String GetRoleByUserId(int userId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var user = connection.Query("select role.roleName from account join role on account.role=role.id where account.id=@uId", new
                {
                    uId = userId
                }).ToList()[0];
                return user.roleName;
            }
        }
    }
}
