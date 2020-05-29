using IoTManager.IDao;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Options;
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
//测试数据库用使用user表,请改为account
namespace IoTManager.Dao
{
    public sealed class MySQLUserDao : IUserDao
    {
        private readonly DatabaseConStr _databaseConStr;
        public MySQLUserDao(IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
        }
        public String Create(UserModel userModel)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int rows = connection.Execute(
                    "INSERT INTO account(userName, displayName, password, email, phoneNumber, remark, role) VALUES (@un, @dn, @p, @e, @pn, @r, 1)",
                    new
                    {
                        un = userModel.UserName,
                        dn = userModel.DisplayName,
                        p = userModel.Password,
                        e = userModel.Email,
                        pn = userModel.PhoneNumber,
                        r = userModel.Remark
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public List<UserModel> Get()
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection
                    .Query<UserModel>("SELECT * FROM account")
                    .ToList();
            }
        }

        public UserModel GetById(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection
                    .Query<UserModel>("SELECT * FROM account WHERE id=@userId", new
                    {
                        userId = id
                    }).FirstOrDefault();
            }
        }

        public UserModel GetByUserName(String userName)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection
                    .Query<UserModel>("SELECT * FROM account WHERE userName=@uName", new
                    {
                        uName = userName
                    }).FirstOrDefault();
            }
        }

        public String Update(int id, UserModel userModel)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int rows = connection
                    .Execute(
                        "UPDATE account SET userName=@un, displayName=@dn, password=@p, email=@e, phoneNumber=@pn, remark=@r, theme=@t, updateTime=CURRENT_TIMESTAMP WHERE id=@userId",
                        new
                        {
                            userId = userModel.Id,
                            un = userModel.UserName,
                            dn = userModel.DisplayName,
                            p = userModel.Password,
                            e = userModel.Email,
                            pn = userModel.PhoneNumber,
                            r = userModel.Remark,
                            t = userModel.Theme
                        });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Delete(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int rows = connection.Execute("DELETE FROM account WHERE id=@userId", new
                {
                    userId = id
                });
                return rows == 1 ? "success" : "error";
            }
        }
        public List<UserModel> GetByName(String userName)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection.Query<UserModel>("select * from account where userName like '%" + userName + "%'")
                    .ToList();
            }
        }

        public String UpdatePassword(String userName, String password)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int rows = connection.Execute("update account set password=@pw where userName=@un",
                    new { pw = password, un = userName });
                return rows == 1 ? "success" : "error";
            }
        }
    }
}
