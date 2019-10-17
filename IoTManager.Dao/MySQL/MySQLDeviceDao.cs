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
        public List<DeviceModel> Get(String searchType, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc", String city = "all", String factory = "all", String workshop = "all")
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
                       "config.configValue deviceType, " +
                       "device.remark, " +
                       "device.lastConnectionTime, " +
                       "device.createTime, " +
                       "device.updateTime, " +
                       "device.pictureRoute, " +
                       "isOnline, " + 
                       "base64Image " +
                       "from device " +
                       "join city on city.id=device.city " +
                       "join factory on factory.id=device.factory " +
                       "join workshop on workshop.id=device.workshop " +
                       "join gateway on gateway.id=device.gatewayId " +
                       "join config on config.id=device.deviceType ";
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
                                                     "device.updateTime, " +
                                                     "device.pictureRoute, " +
                                                     "isOnline, " + 
                                                     "base64Image " +
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
                                                     "device.updateTime, " +
                                                     "device.pictureRoute, " +
                                                     "isOnline, " + 
                                                     "base64Image " +
                                                     "from device " +
                                                     "join city on city.id=device.city " +
                                                     "join factory on factory.id=device.factory " +
                                                     "join workshop on workshop.id=device.workshop " +
                                                     "where device.deviceName like '%"+deviceName+"%'")
                    .ToList();
            }
        }

        public List<DeviceModel> GetByFuzzyDeviceId(String deviceId)
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
                                                     "device.updateTime, " +
                                                     "device.pictureRoute ," +
                                                     "isOnline, " +
                                                     "base64Image " +
                                                     "from device " +
                                                     "join city on city.id=device.city " +
                                                     "join factory on factory.id=device.factory " +
                                                     "join workshop on workshop.id=device.workshop " +
                                                     "where device.hardwareDeviceID like '%"+deviceId+"%'")
                    .ToList();
            }
        }


        public DeviceModel GetByDeviceId(String deviceId)
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
                                                     "device.imageUrl, " +
                                                     "gateway.gatewayName gatewayId, " +
                                                     "mac, " +
                                                     "config.configValue deviceType, " +
                                                     "device.remark, " +
                                                     "device.lastConnectionTime, " +
                                                     "device.createTime, " +
                                                     "device.updateTime, " +
                                                     "device.pictureRoute, " +
                                                     "isOnline, " + 
                                                     "base64Image " +
                                                     "from device " +
                                                     "join city on city.id=device.city " +
                                                     "join factory on factory.id=device.factory " +
                                                     "join workshop on workshop.id=device.workshop " +
                                                     "join gateway on gateway.id=device.gatewayId " +
                                                     "join config on config.id=device.deviceType " +
                                                     "where device.hardwareDeviceID = @hardwareDeviceID", new
                {
                    hardwareDeviceID=deviceId
                }).FirstOrDefault();
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
                int deviceType = connection.Query<int>(
                    "select id from config where configTag=@ct and configValue=@cv", new
                    {
                        ct = "deviceType",
                        cv = deviceModel.DeviceType
                    }).FirstOrDefault();
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
                        dt = deviceType,
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
                int deviceType = connection.Query<int>(
                    "select id from config where configTag=@ct and configValue=@cv", new
                    {
                        ct = "deviceType",
                        cv = deviceModel.DeviceType
                    }).FirstOrDefault();
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
                        "updateTime=CURRENT_TIMESTAMP, " +
                        "pictureRoute=@pr, " +
                        "base64Image=@bi " +
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
                            dt = deviceType,
                            r = deviceModel.Remark,
                            pr = deviceModel.PictureRoute,
                            bi = deviceModel.Base64Image
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
                                                     "device.updateTime, " +
                                                     "device.pictureRoute,  " +
                                                     "isOnline, " +
                                                     "base64Image " +
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

        public long GetDeviceNumber(String searchType, String city="all", String factory="all", String workshop="all")
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String s = "select count(*) number from device";
                if (searchType == "search")
                {
                    if (city != "all")
                    {
                        s += " where device.city in (select id from city where cityName=@cn) ";
                        if (factory != "all")
                        {
                            s += "and device.factory in (select id from factory where factoryName=@fn) ";
                            if (workshop != "all")
                            {
                                s += "and device.workshop in (select id from workshop where workshopName=@wn) ";
                            }
                        }
                    }
                }

                var result = connection.Query(s, new {cn=city, fn=factory, wn=workshop}).FirstOrDefault();
                return result.number;
            }
        }
        
        public List<DeviceModel> GetDeviceByTag(String tag)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String tg = String.Format("\"{0}\"", tag);
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
                                                          "device.updateTime, " +
                                                          "device.pictureRoute, " +
                                                          "isOnline, " +
                                                          "base64Image " +
                                                          "from tag " +
                                                          "join devicetag on devicetag.tagId=tag.id " +
                                                          "join device on device.id=devicetag.deviceId " +
                                                          "join city on city.id=device.city " +
                                                          "join factory on factory.id=device.factory " +
                                                          "join workshop on workshop.id=device.workshop " +
                                                          "where tag.tagName=" + tg).ToList();
            }
        }

        public List<String> GetAllTag()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<String>("select tagName from tag").ToList();
            }
        }

        public Object SetDeviceTag(int deviceId, List<String> tagId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                connection.Execute("delete from devicetag where deviceId=@did", new {did = deviceId});
                foreach (String i in tagId)
                {
                    var tid = connection.Query<String>("select id from tag where tagName=@tn", new {tn = i});
                    connection.Execute("insert into devicetag(deviceId, tagId) values(@did, @tid)", new
                    {
                        did = deviceId,
                        tid = tid
                    });
                }
                return "success";
            }
        }

        public List<DeviceModel> GetByDeviceType(String deviceType)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int typeNum = connection.Query<int>("select id from config where configValue=@cv", new
                {
                    cv = deviceType
                }).FirstOrDefault();
                return connection.Query<DeviceModel>("select device.id, " +
                                                     "hardwareDeviceID, " +
                                                     "deviceName, " +
                                                     "city.cityName as city, " +
                                                     "factory.factoryName as factory, " +
                                                     "workshop.workshopName as workshop, " +
                                                     "deviceState, " +
                                                     "device.imageUrl, " +
                                                     "gateway.gatewayName gatewayId, " +
                                                     "mac, " +
                                                     "config.configValue deviceType, " +
                                                     "device.remark, " +
                                                     "device.lastConnectionTime, " +
                                                     "device.createTime, " +
                                                     "device.updateTime, " +
                                                     "device.pictureRoute, " +
                                                     "isOnline, " +
                                                     "base64Image " +
                                                     "from device " +
                                                     "join city on city.id=device.city " +
                                                     "join factory on factory.id=device.factory " +
                                                     "join workshop on workshop.id=device.workshop " +
                                                     "join gateway on gateway.id=device.gatewayId " +
                                                     "join config on config.id=device.deviceType " +
                                                     "where device.deviceType=@dt", new
                {
                    dt = typeNum
                }).ToList();
            }
        }

        public String SetDeviceOnlineStatus(String id, String status)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("update device set isOnline=@s where device.hardwareDeviceID=@did", new
                {
                    s = status,
                    did = id
                });
                return rows == 1 ? "success" : "error";
            }
        }

        public List<String> GetDeviceTag(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var result = connection
                    .Query<String>("select tagName from devicetag inner join tag on tagId=tag.id where deviceId=@did", new {did=id})
                    .ToList();
                return result;
            }
        }

        public String AddTag(String tagName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var result = connection.Execute("insert into tag(tagName) values(@tn)", new {tn = tagName});
                return result == 1 ? "success" : "error";
            }
        }

        public String DeleteTag(String tagName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var result = connection.Execute("delete from tag where tagName=@tn", new {tn = tagName});
                return result == 1 ? "success" : "error";
            }
        }

        public int FindTagAffiliate(String tagName)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int tagId = connection.Query<int>("select id from tag where tagName=@tn", new {tn = tagName})
                    .FirstOrDefault();
                int result = connection.Query<int>("select count(*) num from (select * from devicetag where tagId=@tid) tmp", new {tid = tagId})
                    .FirstOrDefault();
                return result;
            }
        }
    }
}
