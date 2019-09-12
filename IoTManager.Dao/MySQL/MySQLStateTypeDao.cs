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

        public List<DeviceTypeModel> GetDetailedDeviceType()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                List<DeviceTypeModel> deviceTypes = connection.Query<DeviceTypeModel>(
                    "select config.id, configValue deviceTypeName, offlineTime from config where configTag=@ct", new
                    {
                        ct = "deviceType"
                    }).ToList();
                return deviceTypes;
            }
        }
    }
}