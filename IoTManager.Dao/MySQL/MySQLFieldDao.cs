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
    public sealed class MySQLFieldDao : IFieldDao
    {
        public List<FieldModel> Get(int pageMode = 0, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc")
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String s =
                    "select field.id, fieldName, fieldId, deviceName device, field.updateTime, field.createTime from field join device on field.device=device.id ";
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

                List<FieldModel> fields = connection.Query<FieldModel>(
                    s).ToList();
                return fields;
            }
        }
        public String Create(FieldModel field)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                DeviceModel device = connection.Query<DeviceModel>("select device.id, " +
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
                                                                   "join gateway on gateway.id=device.gatewayId " + 
                                                                   "where deviceName=@dn", new {dn=field.Device})
                    .FirstOrDefault();
                int rows = connection.Execute("insert into field(fieldName, fieldId, device) values (@fn, @fi, @d)", new
                {
                    fn = field.FieldName,
                    fi = field.FieldId,
                    d = device.Id
                });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Update(int id, FieldModel field)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                DeviceModel device = connection.Query<DeviceModel>("select device.id, " +
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
                                                                   "join gateway on gateway.id=device.gatewayId " +
                                                                   "where deviceName=@dn", new {dn = field.Device})
                    .FirstOrDefault();
                int result = connection.Execute("update field set fieldName=@fn, device=@d, updateTime=@ut where id=@i", new
                {
                    i = id,
                    fn = field.FieldName,
                    d = device.Id,
                    ut = device.UpdateTime
                });
                return result == 1 ? "success" : "error";
            }
        }

        public String Delete(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int result = connection.Execute("delete from field where id=@i", new {i = id});
                return result == 1 ? "success" : "error";
            }
        }

        public long GetFieldNumber()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<long>("select count(*) from field join device on field.device=device.id").FirstOrDefault();
            }
        }
        /*zxin-添加  通过deviceId获取所有属性*/
        public List<FieldModel> ListFieldsByDeviceId(string deviceId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                string s = "SELECT * FROM field WHERE field.device IN (SELECT id FROM device WHERE device.hardwareDeviceID=@hid)";
                return connection.Query<FieldModel>(s, new { hid = deviceId }).ToList();
            }
        }
        public List<string> ListFieldIdsByDeviceId(string deviceId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                string s = "SELECT fieldId FROM field WHERE field.device IN (SELECT id FROM device WHERE device.hardwareDeviceID=@hid)";
                return connection.Query<string>(s, new { hid = deviceId }).ToList();
            }
                
        }
    }
}