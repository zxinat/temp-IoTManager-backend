using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
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
                var account = connection.Query("select roleauth.role, roleauth.auth, auth.id, auth.authId, auth.description from account " + 
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
                Console.WriteLine(user.identify);
                Console.WriteLine(user.identify.GetType());
                if (user.identify == 1)
                {
                    var query = connection.Query("select accountauth.operation, accountauth.account, accountauth.auth, auth.id, auth.authId, auth.description from account " + 
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
    }
}