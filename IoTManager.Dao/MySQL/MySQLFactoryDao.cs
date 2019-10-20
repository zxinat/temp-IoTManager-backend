using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using IoTManager.Utility.Serializers;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLFactoryDao : IFactoryDao
    {
        public List<FactoryModel> Get(int pageMode = 0, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc")
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String s = "SELECT factory.id, " +
                           "factoryName, " +
                           "factoryPhoneNumber, " +
                           "factoryAddress, " +
                           "factory.remark, " +
                           "factory.createTime," +
                           " factory.updateTime, " +
                           "city.cityName AS city " +
                           "FROM factory " +
                           "JOIN city ON factory.city=city.id ";
                if (pageMode == 1)
                {
                    if (order != "no" && sortColumn != "no")
                    {
                        String orderBySubsentence = "order by " + sortColumn + " " + order;
                        s += orderBySubsentence;
                    }

                    String limitSubsentence = " limit " + offset.ToString() + "," + limit.ToString();
                    s += limitSubsentence;
                }

                List<FactoryModel> factoryModels = connection
                    .Query<FactoryModel>(s)
                    .ToList();
                return factoryModels;
            }
        }

        public FactoryModel GetById(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection
                    .Query<FactoryModel>("SELECT factory.id, " +
                                         "factoryName, " +
                                         "factoryPhoneNumber, " +
                                         "factoryAddress, " +
                                         "factory.remark, " +
                                         "factory.createTime,"+
                                         " factory.updateTime, " +
                                         "city.cityName AS city " +
                                         "FROM factory " +
                                         "JOIN city ON factory.city=city.id " +
                                         "WHERE factory.id=@factoryId", new
                    {
                        factoryId = id
                    }).FirstOrDefault();
            }
        }

        public String Create(FactoryModel factoryModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                CityModel city = connection
                    .Query<CityModel>("SELECT * FROM city WHERE cityName=@cn", new
                    {
                        cn = factoryModel.City
                    }).FirstOrDefault();
                int rows = connection.Execute(
                    "INSERT INTO factory(factoryName, factoryPhoneNumber, factoryAddress, remark, city) VALUES (@fn, @fpn, @fa, @r, @c)", new
                    {
                        fn = factoryModel.FactoryName,
                        fpn = factoryModel.FactoryPhoneNumber,
                        fa = factoryModel.FactoryAddress,
                        r = factoryModel.Remark,
                        c = city.Id
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Update(int id, FactoryModel factoryModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                CityModel city = connection
                    .Query<CityModel>("SELECT * FROM city WHERE cityName=@cn", new
                    {
                        cn = factoryModel.City
                    }).FirstOrDefault();
                int rows = connection.Execute(
                    "UPDATE factory SET factoryName=@fn, factoryPhoneNumber=@fpn, factoryAddress=@fa, remark=@r, city=@c, updateTime=CURRENT_TIMESTAMP WHERE id=@factoryId",
                    new
                    {
                        fn = factoryModel.FactoryName,
                        fpn = factoryModel.FactoryPhoneNumber,
                        fa = factoryModel.FactoryAddress,
                        r = factoryModel.Remark,
                        c = city.Id,
                        factoryId = id
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Delete(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("DELETE FROM factory WHERE id=@factoryId", new
                {
                    factoryId = id
                });
                return rows == 1 ? "success" : "error";
            }
        }
        public List<FactoryModel> GetAffiliateFactory(String cName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                CityModel city = connection.Query<CityModel>("select * from city where cityName=@cName", new { cName = cName })
                    .FirstOrDefault();
                int cityId = city.Id;
                List<FactoryModel> factories = connection
                    .Query<FactoryModel>("select * from factory where city=@cid", new { cid = cityId }).ToList();
                return factories;
            }
        }
        
        public List<FactoryModel> GetByFactoryName(String factoryName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                List<FactoryModel> factoryModels = connection
                    .Query<FactoryModel>("SELECT factory.id, " +
                                         "factoryName, " +
                                         "factoryPhoneNumber, " +
                                         "factoryAddress, " +
                                         "factory.remark, " +
                                         "factory.createTime,"+
                                         " factory.updateTime, " +
                                         "city.cityName AS city " +
                                         "FROM factory " +
                                         "JOIN city ON factory.city=city.id " +
                                         "WHERE factory.factoryName LIKE '%" + factoryName + "%'")
                    .ToList();
                return factoryModels;
            }
        }

        public int GetFactoryAffiliateWorkshop(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int result = connection.Query<int>("select count(*) number from workshop where factory=@fid", new
                {
                    fid = id
                }).FirstOrDefault();
                return result;
            }
        }

        public int GetFactoryAffiliateDevice(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int result = connection.Query<int>("select count(*) number from device where factory=@fid", new
                {
                    fid = id
                }).FirstOrDefault();
                return result;
            }
        }

        public int GetFactoryAffiliateGateway(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int result = connection.Query<int>("select count(*) number from gateway where factory=@fid", new
                {
                    fid = id
                }).FirstOrDefault();
                return result;
            }
        }

        public long GetFactoryNumber()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<long>("select count(*) from factory").FirstOrDefault();
            }
        }
    }
}