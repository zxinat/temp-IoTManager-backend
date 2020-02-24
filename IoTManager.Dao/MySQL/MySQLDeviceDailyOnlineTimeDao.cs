using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLDeviceDailyOnlineTimeDao: IDeviceDailyOnlineTimeDao
    {
        public List<DeviceDailyOnlineTimeModel> GetAll()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String sql = "select o.id, deviceName, hardwareDeviceID, onlineTime, date, o.createTime timestamp from online_time_daily o inner join device on o.device = device.id";
                var result = connection.Query<DeviceDailyOnlineTimeModel>(sql).ToList();
                return result;
            }
        }

        public String InsertData(DeviceModel device, Double onlineTime)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String sql = "insert into online_time_daily(device, onlineTime, date) values (@d, @ot, @dt)";
                int rows = connection.Execute(sql, new
                {
                    d = device.Id,
                    ot = onlineTime,
                    dt = DateTime.Today
                });
                return rows == 1 ? "success" : "error";
            }
        }

        public List<DeviceDailyOnlineTimeModel> GetOnlineTimeByDevice(String deviceId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String sql = "select o.id, deviceName, hardwareDeviceID, onlineTime, date, o.createTime timestamp from online_time_daily o inner join device on o.device = device.id where hardwareDeviceId = @d";
                var result = connection.Query<DeviceDailyOnlineTimeModel>(sql, new {d = deviceId}).ToList();
                return result;
            } 
        }

        public List<DeviceDailyOnlineTimeModel> SummaryAllDeviceOnlineTime(DateTime startTime, DateTime endTime)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String sql = "select device.deviceName, avg(onlineTime) onlineTime " +
                             "from online_time_daily o " +
                             "inner join device on o.device = device.id " +
                             "where date between @stt and @edt " +
                             "group by device.deviceName";
                var result = connection.Query<DeviceDailyOnlineTimeModel>(sql, new
                {
                    stt = startTime,
                    edt = endTime
                }).ToList();
                return result;
            }
        }
    }
}