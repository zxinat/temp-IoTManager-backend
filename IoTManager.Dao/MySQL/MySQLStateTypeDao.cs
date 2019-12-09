using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLStateTypeDao : IStateTypeDao
    {
        public List<String> GetDeviceType()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection
                    .Query<String>("SELECT configValue FROM config WHERE configTag='deviceType'")
                    .ToList();
            }
        }

        public List<String> GetDeviceState()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection
                    .Query<String>("SELECT configValue FROM config WHERE configTag='deviceState'")
                    .ToList();
            }
        }

        public List<String> GetGatewayType()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection
                    .Query<String>("SELECT configValue FROM config WHERE configTag='gatewayType'")
                    .ToList();
            }
        }

        public List<String> GetGatewayState()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection
                    .Query<String>("SELECT configValue FROM config WHERE configTag='gatewayState'")
                    .ToList();
            }
        }

        public List<DeviceTypeModel> GetDetailedDeviceType(int pageMode = 0, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc")
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String s = "select config.id, configValue deviceTypeName, offlineTime from config where configTag=@ct ";
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

                List<DeviceTypeModel> deviceTypes = connection.Query<DeviceTypeModel>(
                    s, new
                    {
                        ct = "deviceType"
                    }).ToList();
                return deviceTypes;
            }
        }

        public String AddDeviceType(DeviceTypeModel deviceTypeModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var result = connection.Execute(
                    "insert into config(configTag, configValue, offlineTime) values(\'deviceType\', @cv, @ot)", new
                    {
                        cv = deviceTypeModel.DeviceTypeName,
                        ot = deviceTypeModel.OfflineTime
                    });
                return result == 1 ? "success" : "error";
            } 
        }

        public String UpdateDeviceType(int id, DeviceTypeModel deviceTypeModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var result = connection.Execute("update config set configValue=@dtn, offlineTime=@ot where id=@i", new
                {
                    i = id,
                    dtn = deviceTypeModel.DeviceTypeName,
                    ot = deviceTypeModel.OfflineTime
                });
                return result == 1 ? "success" : "error";
            }
        }

        public String DeleteDeviceType(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                var result = connection.Execute("delete from config where id=@i", new {i = id});
                return result == 1 ? "success" : "error";
            }
        }

        public int GetDeviceTypeAffiliateDevice(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                System.Console.WriteLine(id);
                var result = connection.Query<int>("select count(*) num from (select * from device where deviceType=@dt) tmp", new {dt = id}).FirstOrDefault();
                return result;
            }
        }

        public long GetDetailedDeviceTypeNumber()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<long>("select count(*) from config where configTag=@cn", new
                {
                    cn = "deviceType"
                }).FirstOrDefault();
            }
        }

        public DeviceTypeModel GetDeviceTypeByName(String name)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<DeviceTypeModel>(
                    "select config.id id, configValue deviceTypeName, offlineTime from config where configValue=@cv",
                    new
                    {
                        cv = name
                    }).FirstOrDefault();
            }
        }
    }
}