using IoTManager.IDao;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Dapper;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
using Constant = IoTManager.Utility.Constant;
using Microsoft.Extensions.Options;

namespace IoTManager.Dao
{
    public sealed class MySQLCityDao : ICityDao
    {
        private readonly DatabaseConStr _databaseConStr;
        public MySQLCityDao(IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
        }
        public String Create(CityModel cityModel)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int rows = connection.Execute("INSERT INTO city(cityName, remark, longitude, latitude) VALUES (@cn, @r, @lo, @la)", new
                {
                    cn = cityModel.CityName,
                    r = cityModel.Remark,
                    lo = cityModel.longitude,
                    la = cityModel.latitude
                });
                return rows == 1 ? "success" : "error";
            }
        }
        //获取城市信息List
        public List<CityModel> Get(int pageMode = 0, int offset = 0, int limit = 6, String sortColumn = "id", String order = "asc")
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                String s = "select * from city ";
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
                //select * from city order by id asc limit 0,6 
                //按字段id 将结果排序 asc升序/desc降序，limit 后面是限制条件，（0表示忽略开始的0个，6表示查询数据条数）表示从第0条数据往后查6条
                //Console.WriteLine(s);
                return connection
                    .Query<CityModel>(s)
                    .ToList();
            }
        }

        public CityModel GetById(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection
                    .Query<CityModel>("SELECT * FROM city WHERE id = @cityId",
                        new
                        {
                            cityId = id
                        }).FirstOrDefault();
            }
        }

        public String Update(int id, CityModel cityModel)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int rows = connection.Execute("UPDATE city SET cityName=@cn, remark=@r, updateTime=CURRENT_TIMESTAMP WHERE id=@cityId", new
                {
                    cn = cityModel.CityName,
                    r = cityModel.Remark,
                    cityId = cityModel.Id
                });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Delete(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int rows = connection.Execute("DELETE FROM city WHERE id=@cityId", new
                {
                    cityId = id
                });
                return rows == 1 ? "success" : "error";
            }
        }

        public List<CityModel> GetByCityName(String cityName)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection.Query<CityModel>("select * from city where cityName like '%" + cityName + "%'")
                    .ToList();
            }
        }

        public CityModel GetOneCityByName(String cityName)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection.Query<CityModel>("select * from city where cityName=@cn", new {cn = cityName})
                    .FirstOrDefault();
            }
        }

        public object GetThreeLevelMenu()
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                var query = connection.Query<RegionModel>("select * from region").ToList();
                Dictionary<String, String> result = new Dictionary<string, string>();
                foreach (var q in query)
                {
                    result.Add(q.Level, q.RegionName);
                }

                return result;
            }
        }

        public int GetCityAffiliateFactory(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int result = connection.Query<int>("select count(*) number from factory where city=@cid", new
                {
                    cid = id
                }).FirstOrDefault();
                return result;
            }
        }

        public int GetCityAffiliateDevice(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int result = connection.Query<int>("select count(*) number from device where city=@cid", new
                {
                    cid = id
                }).FirstOrDefault();
                return result;
            }
        }

        public int GetCityAffiliateGateway(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int result = connection.Query<int>("select count(*) number from gateway where city=@cid", new
                {
                    cid = id
                }).FirstOrDefault();
                return result;
            }
        }

        public long GetCityNumber()
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection.Query<long>("select count(*) from city").FirstOrDefault();
            }
        }
        /*获取城市名称列表*/
        public List<string> ListCityName()
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection.Query<string>("select city.cityName from city").ToList();
            }
        }
    }
}
