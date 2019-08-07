using IoTManager.IDao;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using IoTManager.Model;
using IoTManager.Utility;

namespace IoTManager.Dao
{
    public sealed class GatewayDao
    {
        public List<GatewayModel> Get(int offset, int limit, int id, int createTime, int updateTime)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<GatewayModel>(
                                                      "select top " + limit.ToString() + " gateway.id, " +
                                                      "hardwareGatewayID," +
                                                      " gatewayName," +
                                                      " gatewayType, " +
                                                      "city.cityName as city, " +
                                                      "factory.factoryName as factory, " +
                                                      "workshop.workshopName as workshop, " +
                                                      "gatewayState, " +
                                                      "imageUrl, " +
                                                      "gateway.remark, " +
                                                      "gateway.lastConnectionTime, " +
                                                      "gateway.createTime, " +
                                                      "gateway.updateTime from gateway " +
                                                      "join city on city.id=gateway.city " +
                                                      "join factory on factory.id=gateway.factory " +
                                                      "join workshop on workshop.id=gateway.workshop "+
                                                      "where gateway.id not in(" + 
                                                      "select top " + offset.ToString() + " gateway.id from gateway)" +
                                                      "and hardwareGatewayID not in(" + 
                                                      "select top " + offset.ToString() + " hardwareGatewayID from gateway)" +
                                                      "and gatewayName not in(" + 
                                                      "select top " + offset.ToString() + " gatewayName from gateway)" +
                                                      "and gatewayType not in(" + 
                                                      "select top " + offset.ToString() + " gatewayType from gateway)" +
                                                      "and city.cityName not in(" + 
                                                      "select top " + offset.ToString() + " city.cityName from gateway)" +
                                                      "and factory.factoryName not in(" + 
                                                      "select top " + offset.ToString() + " factory.factoryName from gateway)" +
                                                      "and workshop.workshopName not in(" + 
                                                      "select top " + offset.ToString() + " workshop.workshopName from gateway)" +
                                                      "and gatewayState not in(" + 
                                                      "select top " + offset.ToString() + " gatewayState from gateway)" +
                                                      "and imageUrl not in(" + 
                                                      "select top " + offset.ToString() + " imageUrl from gateway)" +
                                                      "and gateway.remark not in(" + 
                                                      "select top " + offset.ToString() + " gateway.remark from gateway)" +
                                                      "and gateway.lastConnectionTime not in(" + 
                                                      "select top " + offset.ToString() + " gateway.lastConnectionTime from gateway)" +
                                                      "and gateway.createTime not in(" + 
                                                      "select top " + offset.ToString() + " gateway.createTime from gateway)" +
                                                      "and gateway.updateTime not in(" + 
                                                      "select top " + offset.ToString() + " gateway.updateTime from gateway)" )
                                                      
                    .ToList();
            }
        }

        public GatewayModel GetById(int id)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<GatewayModel>("select gateway.id, " +
                                                      "hardwareGatewayID," +
                                                      " gatewayName," +
                                                      " gatewayType, " +
                                                      "city.cityName as city, " +
                                                      "factory.factoryName as factory, " +
                                                      "workshop.workshopName as workshop, " +
                                                      "gatewayState, " +
                                                      "imageUrl, " +
                                                      "gateway.remark, " +
                                                      "gateway.lastConnectionTime, " +
                                                      "gateway.createTime, " +
                                                      "gateway.updateTime from gateway " +
                                                      "join city on city.id=gateway.city " +
                                                      "join factory on factory.id=gateway.factory " +
                                                      "join workshop on workshop.id=gateway.workshop " +
                                                      "where gateway.id=@gatewayId", new
                {
                    gatewayId = id
                }).FirstOrDefault();
            }
        }

        public String Create(GatewayModel gatewayModel)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                CityModel city = connection.Query<CityModel>(
                    "SELECT * FROM city WHERE city.cityName=@cn", new
                    {
                        cn = gatewayModel.City
                    }).FirstOrDefault();
                FactoryModel factory = connection.Query<FactoryModel>(
                    "SELECT * FROM factory WHERE factory.factoryName=@fn", new
                    {
                        fn = gatewayModel.Factory
                    }).FirstOrDefault();
                WorkshopModel workshop = connection.Query<WorkshopModel>(
                    "SELECT * FROM workshop WHERE workshop.workshopName=@wn", new
                    {
                        wn = gatewayModel.Workshop
                    }).FirstOrDefault();
                int rows = connection.Execute(
                    "INSERT INTO gateway(hardwareGatewayID, gatewayName, city, factory, workshop, gatewayType, gatewayState, imageUrl, remark)" +
                    " VALUES(@hgid, @gn, @c, @f, @w, @gt, @gs, @iu, @r)", new
                    {
                        hgid = gatewayModel.HardwareGatewayId,
                        gn = gatewayModel.GatewayName,
                        c = city.Id,
                        f = factory.Id,
                        w = workshop.Id,
                        gt = gatewayModel.GatewayType,
                        gs = gatewayModel.GatewayState,
                        iu = gatewayModel.ImageUrl,
                        r = gatewayModel.Remark
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Update(int id, GatewayModel gatewayModel)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                CityModel city = connection.Query<CityModel>(
                    "SELECT * FROM city WHERE city.cityName=@cn", new
                    {
                        cn = gatewayModel.City
                    }).FirstOrDefault();
                FactoryModel factory = connection.Query<FactoryModel>(
                    "SELECT * FROM factory WHERE factory.factoryName=@fn", new
                    {
                        fn = gatewayModel.Factory
                    }).FirstOrDefault();
                WorkshopModel workshop = connection.Query<WorkshopModel>(
                    "SELECT * FROM workshop WHERE workshop.workshopName=@wn", new
                    {
                        wn = gatewayModel.Workshop
                    }).FirstOrDefault();
                int rows = connection.Execute(
                    "UPDATE gateway SET hardwareGatewayID=@hgid, gatewayName=@gn, city=@c, factory=@f, workshop=@w, gatewayType=@gt, gatewayState=@gs, imageUrl=@iu, remark=@r, updateTime=CURRENT_TIMESTAMP WHERE id=@gatewayId",
                    new
                    {
                        gatewayId = id,
                        hgid = gatewayModel.HardwareGatewayId,
                        gn = gatewayModel.GatewayName,
                        c = city.Id,
                        f = factory.Id,
                        w = workshop.Id,
                        gt = gatewayModel.GatewayType,
                        gs = gatewayModel.GatewayState,
                        iu = gatewayModel.ImageUrl,
                        r = gatewayModel.Remark
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Delete(int id)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("DELETE FROM gateway WHERE id=@gatewayId", new
                {
                    gatewayId = id
                });
                return rows == 1 ? "success" : "error";
            }
        }

        public List<GatewayModel> GetByWorkshop(String city, String factory, String workshop)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<GatewayModel>("select gateway.id, " +
                                                      "hardwareGatewayID," +
                                                      " gatewayName," +
                                                      " gatewayType, " +
                                                      "city.cityName as city, " +
                                                      "factory.factoryName as factory, " +
                                                      "workshop.workshopName as workshop, " +
                                                      "gatewayState, " +
                                                      "imageUrl, " +
                                                      "gateway.remark, " +
                                                      "gateway.lastConnectionTime, " +
                                                      "gateway.createTime, " +
                                                      "gateway.updateTime from gateway " +
                                                      "join city on city.id=gateway.city " +
                                                      "join factory on factory.id=gateway.factory " +
                                                      "join workshop on workshop.id=gateway.workshop " +
                                                      "where gateway.workshop in (select id from workshop where workshopName=@wn) " +
                                                      "and gateway.factory in (select id from factory where factoryName=@fn) " +
                                                      "and gateway.city in (select id from city where cityName=@cn)", new
                    {
                        cn = city,
                        fn = factory,
                        wn = workshop
                    })
                    .ToList();
            }
        }

        public int BatchDelete(int[] ids)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = 0;
                foreach (int i in ids)
                {
                    connection.Execute("delete from gateway where gateway.id=@gid", new {gid = i});
                    rows = rows + 1;
                }

                return rows;
            }
        }

        public String CreateGatewayType(String gatewayType)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("insert into config(configTag, configValue)  values ('gatewayType', @gt)",
                    new
                    {
                        gt = gatewayType
                    });
                return rows == 1 ? "success" : "error";
            }
        }
    }
}
