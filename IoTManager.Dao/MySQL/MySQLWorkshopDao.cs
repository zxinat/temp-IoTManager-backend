using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLWorkshopDao : IWorkshopDao
    {
        public List<WorkshopModel> Get(int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc")
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
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
                if (order != "no" && sortColumn != "no")
                {
                    String orderBySubsentence = "order by " + sortColumn + " " + order;
                    s += orderBySubsentence;
                }

                String limitSubsentence = " limit " + offset.ToString() + "," + limit.ToString();
                s += limitSubsentence;
                
                List<WorkshopModel> workshopModels = connection
                    .Query<WorkshopModel>(s)
                    .ToList();
                return workshopModels;
            }
        }

        public WorkshopModel GetById(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
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
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                FactoryModel factory = connection
                    .Query<FactoryModel>("SELECT * FROM factory WHERE factoryName=@fn", new
                    {
                        fn = workshopModel.Factory
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
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
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
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("DELETE FROM workshop WHERE id=@workshopId", new
                {
                    workshopId = id
                });
                return rows == 1 ? "success" : "error";
            }
        }
        public List<WorkshopModel> GetAffiliateWorkshop(String fName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                FactoryModel factory = connection
                    .Query<FactoryModel>("select * from factory where factoryName=@fn", new { fn = fName })
                    .FirstOrDefault();
                int factoryId = factory.Id;
                List<WorkshopModel> workshops = connection
                    .Query<WorkshopModel>("select * from workshop where factory=@fid", new { fid = factoryId }).ToList();
                return workshops;
            }
        }

        public List<WorkshopModel> GetByWorkshopName(String workshopName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
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
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
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
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int result = connection.Query<int>("select count(*) number from gateway where workshop=@wid", new
                {
                    wid = id
                }).FirstOrDefault();
                return result;
            }
        }
    }
}