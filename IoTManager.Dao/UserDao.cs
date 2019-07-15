using IoTManager.IDao;
using System;
using System.Collections.Generic;
//using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
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
    public sealed class UserDao:IUserDao
    {
        public String Create(UserModel userModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute(
                    "INSERT INTO user(userName, displayName, password, email, phoneNumber, remark) VALUES (@un, @dn, @p, @e, @pn, @r)",
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
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection
                    .Query<UserModel>("SELECT * FROM user")
                    .ToList();
            }
        }

        public UserModel GetById(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection
                    .Query<UserModel>("SELECT * FROM user WHERE id=@userId", new
                    {
                        userId = id
                    }).FirstOrDefault();
            }
        }

        public UserModel GetByUserName(String userName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection
                    .Query<UserModel>("SELECT * FROM user WHERE userName=@uName", new
                    {
                        uName = userName
                    }).FirstOrDefault();
            }
        }

        public String Update(int id, UserModel userModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection
                    .Execute(
                        "UPDATE user SET userName=@un, displayName=@dn, password=@p, email=@e, phoneNumber=@pn, remark=@r, updateTime=CURRENT_TIMESTAMP WHERE id=@userId",
                        new
                        {
                            userId = userModel.Id,
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

        public String Delete(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("DELETE FROM user WHERE id=@userId", new
                {
                    userId = id
                });
                return rows == 1 ? "success" : "error";
            }
        }
    }
}
