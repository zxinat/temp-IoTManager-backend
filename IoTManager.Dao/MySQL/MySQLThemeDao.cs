using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLThemeDao : IThemeDao
    {
        public List<ThemeModel> Get()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<ThemeModel>("select * from theme").ToList();
            }
        }

        public ThemeModel GetById(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<ThemeModel>("select * from theme where id=@i", new
                {
                    i = id
                }).ToList()[0];
            }
        }

        public String Create(ThemeModel themeModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("insert into theme(name, first, second, third) values (@n, @f, @s, @t)",
                    new
                    {
                        n = themeModel.Name,
                        f = themeModel.First,
                        s = themeModel.Second,
                        t = themeModel.Third
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Update(int id, ThemeModel themeModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection
                    .Execute(
                        "update theme set name=@n, first=@f, second=@s, third=@t where id=@i", new
                        {
                            n = themeModel.Name,
                            f = themeModel.First,
                            s = themeModel.Second,
                            t = themeModel.Third,
                            i = id
                        });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Delete(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("delete from theme where id=@i", new
                {
                    i = id
                });
                return rows == 1 ? "success" : "error";
            }
        }

        public ThemeModel GetByUserId(int userId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<ThemeModel>(
                    "select theme.id, theme.name, theme.first, theme.second, theme.third from theme join account on account.theme = theme.id where account.id=@userId",
                    new
                    {
                        userId = userId
                    }).ToList()[0];
            }
        }
    }
}