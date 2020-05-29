using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Model.DataReceiver;
using IoTManager.Utility;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLWorkshopDao : IWorkshopDao
    {
        private readonly DatabaseConStr _databaseConStr;
        public MySQLWorkshopDao(IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
        }
        public List<WorkshopModel> Get(int pageMode = 0, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc")
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                /*
                String s = "SELECT workshop.id, " +
                           "workshopName, " +
                           "workshopPhoneNumber, " +
                           "workshopAddress, " +
                           "workshop.remark, " +
                           "workshop.createTime, " +
                           "workshop.updateTime, " +
                           "factory.factoryName AS factory " +
                           "FROM workshop JOIN factory " +
                           "ON workshop.factory=factory.id ";
                 */
                string s = "SELECT w.id," +
                    "w.workshopName, " +
                    "w.workshopPhoneNumber, " +
                    "w.workshopAddress, " +
                    "w.remark, " +
                    "w.createTime, " +
                    "w.updateTime, " +
                    "f.factoryName factory," +
                    "c.cityName city " +
                    "FROM workshop w," +
                    "factory f," +
                    "city c " +
                    "WHERE w.factory=f.id " +
                    "AND f.city=c.id ";
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

                List<WorkshopModel> workshopModels = connection
                    .Query<WorkshopModel>(s)
                    .ToList();
                return workshopModels;
            }
        }

        public WorkshopModel GetById(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                WorkshopModel workshopModel = connection
                    .Query<WorkshopModel>("SELECT workshop.id, " +
                                          "workshopName, " +
                                          "workshopPhoneNumber, " +
                                          "workshopAddress, " +
                                          "workshop.remark, " +
                                          "workshop.createTime, " +
                                          "workshop.updateTime, " +
                                          "factory.factoryName AS factory " +
                                          "FROM workshop JOIN factory " +
                                          "ON workshop.factory=factory.id " +
                                          "WHERE workshop.id=@workshopId", new
                                          {
                                              workshopId = id
                                          }).FirstOrDefault();
                return workshopModel;
            }
        }

        public String Create(WorkshopModel workshopModel)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                /*
                FactoryModel factory = connection
                    .Query<FactoryModel>("SELECT * FROM factory WHERE factoryName=@fn", new
                    {
                        fn = workshopModel.Factory
                    }).FirstOrDefault();
                */
                FactoryModel factory = connection
                    .Query<FactoryModel>("SELECT * FROM factory WHERE factoryName=@fn AND factory.city IN (SELECT city.id FROM city WHERE city.cityName=@cn)", new
                    {
                        fn = workshopModel.Factory,
                        cn = workshopModel.City
                    }).FirstOrDefault();
                int rows = connection
                    .Execute(
                        "INSERT INTO workshop(workshopName, workshopPhoneNumber, workshopAddress, remark, factory) VALUES (@wn, @wpn, @wa, @r, @f)", new
                        {
                            wn = workshopModel.WorkshopName,
                            wpn = workshopModel.WorkshopPhoneNumber,
                            wa = workshopModel.WorkshopAddress,
                            r = workshopModel.Remark,
                            f = factory.Id
                        });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Update(int id, WorkshopModel workshopModel)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                FactoryModel factory = connection
                    .Query<FactoryModel>("SELECT * FROM factory WHERE factoryName=@fn", new
                    {
                        fn = workshopModel.Factory
                    }).FirstOrDefault();
                int rows = connection
                    .Execute(
                        "UPDATE workshop SET workshopName=@wn, workshopPhoneNumber=@wpn, workshopAddress=@wa, remark=@r, factory=@f, updateTime=CURRENT_TIMESTAMP WHERE id=@workshopId",
                        new
                        {
                            wn = workshopModel.WorkshopName,
                            wpn = workshopModel.WorkshopPhoneNumber,
                            wa = workshopModel.WorkshopAddress,
                            r = workshopModel.Remark,
                            f = factory.Id,
                            workshopId = id
                        });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Delete(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int rows = connection.Execute("DELETE FROM workshop WHERE id=@workshopId", new
                {
                    workshopId = id
                });
                return rows == 1 ? "success" : "error";
            }
        }
        public List<WorkshopModel> GetAffiliateWorkshop(String cName,String fName)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                /*
                FactoryModel factory = connection
                    .Query<FactoryModel>("select * from factory where factoryName=@fn", new { fn = fName })
                    .FirstOrDefault();
                int factoryId = factory.Id;
                List<WorkshopModel> workshops = connection
                    .Query<WorkshopModel>("select * from workshop where factory=@fid", new { fid = factoryId }).ToList();
                */
                /*zxin-添加所属城市筛选*/
                List<WorkshopModel> workshops = connection
                    .Query<WorkshopModel>("select * from workshop WHERE workshop.factory IN (SELECT factory.id FROM factory WHERE factoryName=@fn AND factory.city IN(SELECT city.id FROM city WHERE city.cityName=@cn))", new
                    { fn = fName, cn = cName }).ToList();
                return workshops;
            }
        }
        /*zxin-获取实验室名称列表：输入：城市、实验楼，输出实验室名称列表*/
        public List<WorkshopTreeModel> ListWorkshopLoaction()
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                List<WorkshopTreeModel> workshopLocation = connection
                    .Query<WorkshopTreeModel>("SELECT w.workshopName,f.factoryName,c.cityName " +
                    "FROM workshop w,factory f,city c WHERE f.id=w.factory AND c.id=f.city").ToList();
                return workshopLocation;
            }
        }
        public List<string> ListWorkshopNames(string cName,string fName)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                List<string> workshopNameList = connection
                    .Query<string>("SELECT workshopName FROM workshop,factory,city " +
                    "WHERE workshop.factory=factory.id and factory.city=city.id and factory.factoryName=@fn and cityName=@cn", 
                    new { fn = fName, cn = cName }).ToList();
                return workshopNameList;
            }
                
        }
        public List<WorkshopModel> GetByWorkshopName(String workshopName)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                List<WorkshopModel> workshopModels = connection
                    .Query<WorkshopModel>("SELECT workshop.id, " +
                                          "workshopName, " +
                                          "workshopPhoneNumber, " +
                                          "workshopAddress, " +
                                          "workshop.remark, " +
                                          "workshop.createTime, " +
                                          "workshop.updateTime, " +
                                          "factory.factoryName AS factory " +
                                          "FROM workshop JOIN factory " +
                                          "ON workshop.factory=factory.id " + 
                                          "WHERE workshop.workshopName LIKE '%" + workshopName + "%'")
                    .ToList();
                return workshopModels;
            }
        }

        public int GetWorkshopAffiliateDevice(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int result = connection.Query<int>("select count(*) number from device where workshop=@wid", new
                {
                    wid = id
                }).FirstOrDefault();
                return result;
            }
        }

        public int GetWorkshopAffiliateGateway(int id)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                int result = connection.Query<int>("select count(*) number from gateway where workshop=@wid", new
                {
                    wid = id
                }).FirstOrDefault();
                return result;
            }
        }
        /*获取实验室所属地列表：返回：城市名、实验楼名、实验室名 列表，[{"上海","1号楼","天平实验室"}]*/
        public long GetWorkshopNumber()
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                return connection.Query<long>("select count(*) from workshop").FirstOrDefault();
            }
        }
    }
}