using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MongoDB.Bson;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLAuthDao : IAuthDao
    {
        public List<String> GetAuthByUserId(int userId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var account = connection.Query(
                        "select roleauth.role, roleauth.auth, auth.id, auth.authId, auth.description from account " +
                        "join roleauth on account.role=roleauth.role " +
                        "join auth on roleauth.auth=auth.id " +
                        "where account.id=@uid", new
                        {
                            uid = userId
                        })
                    .ToList();
                var user = connection.Query("select identify from account where account.id=@uid", new
                {
                    uid = userId
                }).ToList()[0];
                if (user.identify == 1)
                {
                    var query = connection.Query(
                            "select accountauth.operation, accountauth.account, accountauth.auth, auth.id, auth.authId, auth.description from account " +
                            "join accountauth on account.id=accountauth.account " +
                            "join auth on accountauth.auth=auth.id " +
                            "where account.id=@uid", new
                            {
                                uid = userId
                            })
                        .ToList();
                    foreach (var q in query)
                    {
                        if (q.operation.Equals("ADD"))
                        {
                            account.Add(q);
                        }
                        else
                        {
                            account.Remove(account.Find(qq => qq.id == q.auth));
                        }
                    }
                }

                List<String> accountAuth = new List<string>();
                foreach (var aa in account)
                {
                    accountAuth.Add(aa.authId);
                }

                return accountAuth;
            }
        }

        public List<String> GetAuthByRoleId(int roleId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String s = String.Format(
                    "select roleauth.role, roleauth.auth, auth.id, auth.authId, auth.description from role " +
                    "join roleauth on role.id=roleauth.role " +
                    "join auth on roleauth.auth=auth.id " +
                    "where role.id={0}", roleId);
                Console.WriteLine(roleId);
                var account = connection.Query(s).ToList();
                List<String> roleAuth = new List<string>();
                foreach (var aa in account)
                {
                    roleAuth.Add(aa.authId);
                }

                return roleAuth;
            }
        }

        public List<String> GetAllAuth()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var auth = connection.Query("select authId from auth").ToList();
                List<String> authList = new List<string>();
                foreach (var a in auth)
                {
                    authList.Add(a.authId);
                }

                return authList;
            }
        }
    }
}