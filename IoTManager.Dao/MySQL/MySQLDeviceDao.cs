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

namespace IoTManager.Dao
{
    public sealed class MySQLDeviceDao : IDeviceDao
    {
        public List<DeviceModel> Get(String searchType, int offset, int limit, String sortColumn, String order, String city, String factory, String workshop)
        {
            string s = "select device.id, " +
                       "hardwareDeviceID, " +
                       "deviceName, " +
                       "city.cityName as city, " +
                       "factory.factoryName as factory, " +
                       "workshop.workshopName as workshop, " +
                       "deviceState, " +
                       "device.imageUrl, " +
                       "gateway.gatewayName gatewayId, " +
                       "mac, " +
                       "deviceType, " +
                       "device.remark, " +
                       "device.lastConnectionTime, " +
                       "device.createTime, " +
                       "device.updateTime " +
                       "from device " +
                       "join city on city.id=device.city " +
                       "join factory on factory.id=device.factory " +
                       "join workshop on workshop.id=device.workshop " +
                       "join gateway on gateway.id=device.gatewayId ";
            if (searchType == "search")
            {
                if (city != "all")
                {
                    s += "where device.city in (select id from city where cityName=@cn) ";
                    if (factory != "all")
                    {
                        s += "and device.factory in (select id from factory where factoryName=@fn) ";
                        if (workshop != "all")
                        {
                            s += "and device.workshop in (select id from workshop where workshopName=@wn) ";
                        }
                    }
                }

                if (order != "no" && sortColumn != "no")
                {
                    String orderBySubsentence = "order by " + sortColumn + " " + order;
                    s += orderBySubsentence;
                }

                String limitSubsentence = " limit " + offset.ToString() + "," + limit.ToString();
                s += limitSubsentence;
            }

            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<DeviceModel>(s, new {cn=city, fn=factory, wn=workshop})
                    .ToList();
            }
            
        }

        public DeviceModel GetById(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<DeviceModel>("select device.id, " +
                                                     "hardwareDeviceID, " +
                                                     "deviceName, " +
                                                     "city.cityName as city, " +
                                                     "factory.factoryName as factory, " +
                                                     "workshop.workshopName as workshop, " +
                                                     "deviceState, " +
                                                     "imageUrl, " +
                                                     "gatewayID, " +
                                                     "mac, " +
                                                     "deviceType, " +
                                                     "device.remark, " +
                                                     "lastConnectionTime, " +
                                                     "device.createTime, " +
                                                     "device.updateTime " +
                                                     "from device " +
                                                     "join city on city.id=device.city " +
                                                     "join factory on factory.id=device.factory " +
                                                     "join workshop on workshop.id=device.workshop " +
                                                     "where device.id=@deviceId", new
                {
                    deviceId = id
                }).FirstOrDefault();
            }
        }

        public List<DeviceModel> GetByDeviceName(String deviceName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<DeviceModel>("select device.id, " +
                                                     "hardwareDeviceID, " +
                                                     "deviceName, " +
                                                     "city.cityName as city, " +
                                                     "factory.factoryName as factory, " +
                                                     "workshop.workshopName as workshop, " +
                                                     "deviceState, " +
                                                     "imageUrl, " +
                                                     "gatewayID, " +
                                                     "mac, " +
                                                     "deviceType, " +
                                                     "device.remark, " +
                                                     "lastConnectionTime, " +
                                                     "device.createTime, " +
                                                     "device.updateTime " +
                                                     "from device " +
                                                     "join city on city.id=device.city " +
                                                     "join factory on factory.id=device.factory " +
                                                     "join workshop on workshop.id=device.workshop " +
                                                     "where device.deviceName like '%"+deviceName+"%'")
                    .ToList();
            }
        }

        public List<DeviceModel> GetByDeviceId(String deviceId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<DeviceModel>("select device.id, " +
                                                     "hardwareDeviceID, " +
                                                     "deviceName, " +
                                                     "city.cityName as city, " +
                                                     "factory.factoryName as factory, " +
                                                     "workshop.workshopName as workshop, " +
                                                     "deviceState, " +
                                                     "imageUrl, " +
                                                     "gatewayID, " +
                                                     "mac, " +
                                                     "deviceType, " +
                                                     "device.remark, " +
                                                     "lastConnectionTime, " +
                                                     "device.createTime, " +
                                                     "device.updateTime " +
                                                     "from device " +
                                                     "join city on city.id=device.city " +
                                                     "join factory on factory.id=device.factory " +
                                                     "join workshop on workshop.id=device.workshop " +
                                                     "where device.hardwareDeviceID like '%"+deviceId+"%'")
                    .ToList();
            }
        }

        public String Create(DeviceModel deviceModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                CityModel city = connection.Query<CityModel>(
                    "SELECT * FROM city WHERE city.cityName=@cn", new
                    {
                        cn = deviceModel.City
                    }).FirstOrDefault();
                FactoryModel factory = connection.Query<FactoryModel>(
                    "SELECT * FROM factory WHERE factory.factoryName=@fn", new
                    {
                        fn = deviceModel.Factory
                    }).FirstOrDefault();
                WorkshopModel workshop = connection.Query<WorkshopModel>(
                    "SELECT * FROM workshop WHERE workshop.workshopName=@wn", new
                    {
                        wn = deviceModel.Workshop
                    }).FirstOrDefault();
                GatewayModel gateway = connection.Query<GatewayModel>("select * from gateway where gatewayName=@gn",
                    new {gn = deviceModel.GatewayId}).FirstOrDefault();
                int rows = connection.Execute(
                    "INSERT INTO "+ 
                    "device(hardwareDeviceID, deviceName, city, factory, workshop, deviceState, imageUrl, gatewayId, mac, deviceType, remark)" +
                    " VALUES (@hdid, @dn, @c, @f, @w, @ds, @iu, @gid, @m, @dt, @r)", new
                    {
                        hdid = deviceModel.HardwareDeviceId,
                        dn = deviceModel.DeviceName,
                        c = city.Id,
                        f = factory.Id,
                        w = workshop.Id,
                        ds = deviceModel.DeviceState,
                        iu = deviceModel.ImageUrl,
                        gid = gateway.Id,
                        m = deviceModel.Mac,
                        dt = deviceModel.DeviceType,
                        r = deviceModel.Remark
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Update(int id, DeviceModel deviceModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                CityModel city = connection.Query<CityModel>(
                    "SELECT * FROM city WHERE city.cityName=@cn", new
                    {
                        cn = deviceModel.City
                    }).FirstOrDefault();
                FactoryModel factory = connection.Query<FactoryModel>(
                    "SELECT * FROM factory WHERE factory.factoryName=@fn", new
                    {
                        fn = deviceModel.Factory
                    }).FirstOrDefault();
                WorkshopModel workshop = connection.Query<WorkshopModel>(
                    "SELECT * FROM workshop WHERE workshop.workshopName=@wn", new
                    {
                        wn = deviceModel.Workshop
                    }).FirstOrDefault();
                GatewayModel gateway = connection.Query<GatewayModel>("select * from gateway where gatewayName=@gn",
                    new {gn = deviceModel.GatewayId}).FirstOrDefault();
                int rows = connection
                    .Execute(
                        "UPDATE device "+
                        "SET hardwareDeviceID=@hdid, " +
                        "deviceName=@dn, " +
                        "city=@c, " +
                        "factory=@f, " +
                        "workshop=@w, " +
                        "deviceState=@ds, " +
                        "imageUrl=@iu, " +
                        "gatewayId=@gid, " +
                        "mac=@m, " +
                        "deviceType=@dt, " +
                        "remark=@r, " +
                        "updateTime=CURRENT_TIMESTAMP " +
                        "WHERE device.id=@deviceId",
                        new
                        {
                            deviceId = deviceModel.Id,
                            hdid = deviceModel.HardwareDeviceId,
                            dn = deviceModel.DeviceName,
                            c = city.Id,
                            f = factory.Id,
                            w = workshop.Id,
                            ds = deviceModel.DeviceState,
                            iu = deviceModel.ImageUrl,
                            gid = gateway.Id,
                            m = deviceModel.Mac,
                            dt = deviceModel.DeviceType,
                            r = deviceModel.Remark
                        });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Delete(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("DELETE FROM device WHERE device.id=@deviceId", new
                {
                    deviceId = id
                });
                return rows == 1 ? "success" : "error";
            }
        }

        public int BatchDelete(int[] ids)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = 0;
                foreach (int i in ids)
                {
                    connection.Execute("DELETE FROM device WHERE device.id=@deviceId", new
                    {
                        deviceId = i
                    });
                    rows = rows + 1;
                }
                return rows;
            }
        }

        public List<DeviceModel> GetByWorkshop(String city, String factory, String workshop)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<DeviceModel>("select device.id, " +
                                                     "hardwareDeviceID, " +
                                                     "deviceName, " +
                                                     "city.cityName as city, " +
                                                     "factory.factoryName as factory, " +
                                                     "workshop.workshopName as workshop, " +
                                                     "deviceState, " +
                                                     "imageUrl, " +
                                                     "gatewayID, " +
                                                     "mac, " +
                                                     "deviceType, " +
                                                     "device.remark, " +
                                                     "lastConnectionTime, " +
                                                     "device.createTime, " +
                                                     "device.updateTime " +
                                                     "from device " +
                                                     "join city on city.id=device.city " +
                                                     "join factory on factory.id=device.factory " +
                                                     "join workshop on workshop.id=device.workshop " +
                                                     "where device.workshop in (select id from workshop where workshopName=@wn) " +
                                                     "and device.factory in (select id from factory where factoryName=@fn) " +
                                                     "and device.city in (select id from city where cityName=@cn)", new
                                                     {
                                                         cn = city,
                                                         fn = factory,
                                                         wn = workshop
                                                     })
                    .ToList();
            }
        }
        public int GetDeviceAmount()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<int>("select count(*) from device").FirstOrDefault();
            }
        }

        public List<object> GetDeviceTree(String city, String factory)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                CityModel c = connection.Query<CityModel>("select * from city where cityName=@cn", new { cn = city })
                    .FirstOrDefault();
                FactoryModel fac = connection
                    .Query<FactoryModel>("select * from factory where factoryName=@fn and city=@cid", new { fn = factory, cid = c.Id })
                    .FirstOrDefault();
                List<WorkshopModel> workshops =
                    connection.Query<WorkshopModel>("select * from workshop where factory=@fid", new { fid = fac.Id })
                        .ToList();
                List<object> result = new List<object>();
                foreach (WorkshopModel w in workshops)
                {
                    List<DeviceModel> devices = connection
                        .Query<DeviceModel>("select * from device where workshop=@wid", new { wid = w.Id })
                        .ToList();
                    List<object> deviceResult = new List<object>();
                    foreach (DeviceModel d in devices)
                    {
                        deviceResult.Add(new { label = d.DeviceName, id = d.Id });
                    }
                    result.Add(new { label = w.WorkshopName, children = deviceResult });
                }

                return result;
            }
        }
        public String CreateDeviceType(String deviceType)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("insert into config(configTag, configValue) values ('deviceType', @dt)",
                    new
                    {
                        dt = deviceType
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public long GetDeviceNumber()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var result = connection.Query("select count(*) number from device").FirstOrDefault();
                return result.number;
            }
        }
    }
}
