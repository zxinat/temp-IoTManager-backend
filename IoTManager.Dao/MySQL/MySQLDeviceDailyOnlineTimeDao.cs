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
        public string InsertData(DeviceModel device,double onlineTime,DateTime date)
        {
            DeviceDailyOnlineTimeModel deviceDailyOnlineTime = GetDeviceDailyOnlineTime(device.DeviceName, date);
            if(deviceDailyOnlineTime==null)
            {
                using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
                {
                    string sql = "insert into online_time_daily(device, onlineTime, date) values (@d, @ot, @dt)";
                    int rows = connection.Execute(sql, new
                    {
                        d = device.Id,
                        ot = onlineTime,
                        dt = date.ToString()
                    });
                    return rows == 1 ? "success" : "error";
                }
            }
            else
            {
                using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
                {
                    string sql = "UPDATE  online_time_daily SET onlineTime=@olt WHERE id=@id";
                    int rows = connection.Execute(sql, new
                    {
                        id= deviceDailyOnlineTime.Id,
                        olt= onlineTime
                    });
                    return rows == 1 ? "success" : "error";
                }
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

        public List<DeviceDailyOnlineTimeModel> GetDeviceOnlineTimeByTime(DateTime startTime, DateTime endTime)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String sql = "select o.id, device.hardwareDeviceID, device.deviceName, onlineTime, date, o.createTime timestamp " +
                             "from online_time_daily o " +
                             "inner join device on o.device = device.id " +
                             "where date between @stt and @edt ";
                var result = connection.Query<DeviceDailyOnlineTimeModel>(sql, new
                {
                    stt = startTime,
                    edt = endTime
                }).ToList();
                return result;
            }
        }
        /*通过deviceName和date获取日在线时间*/
        public DeviceDailyOnlineTimeModel GetDeviceDailyOnlineTime(string deviceName,DateTime Date)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String sql = "SELECT * FROM online_time_daily WHERE device IN (SELECT id FROM device WHERE deviceName=@dn) AND date=@date";
                var result = connection.Query<DeviceDailyOnlineTimeModel>(sql, new
                {
                    dn = deviceName,
                    date = Date.ToString()
                }).FirstOrDefault();
                return result;
            }
        }
    }
}