using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace IoTManager.Dao
{
    public sealed class MySQLDeviceDailyOnlineTimeDao: IDeviceDailyOnlineTimeDao
    {
        private readonly DatabaseConStr _databaseConStr;
        public MySQLDeviceDailyOnlineTimeDao(IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
        }
        public List<DeviceDailyOnlineTimeModel> GetAll()
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                String sql = "select o.id, deviceName, hardwareDeviceID, onlineTime, date, o.createTime timestamp from online_time_daily o inner join device on o.device = device.id";
                var result = connection.Query<DeviceDailyOnlineTimeModel>(sql).ToList();
                return result;
            }
        }

        public String InsertData(DeviceModel device, Double onlineTime)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
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
                using (var connection = new MySqlConnection(_databaseConStr.MySQL))
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
                using (var connection = new MySqlConnection(_databaseConStr.MySQL))
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
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                String sql = "select o.id, deviceName, hardwareDeviceID, onlineTime, date, o.createTime timestamp from online_time_daily o inner join device on o.device = device.id where hardwareDeviceId = @d";
                var result = connection.Query<DeviceDailyOnlineTimeModel>(sql, new {d = deviceId}).ToList();
                return result;
            } 
        }

        public List<DeviceDailyOnlineTimeModel> GetDeviceOnlineTimeByTime(DateTime startTime, DateTime endTime)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
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
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
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
        /*获取设备一段时间内在线总时长*/
        public double GetTotalMinutesOnline(string deviceId,DateTime startTime,DateTime endTime)
        {
            string sd = startTime.ToLocalTime().ToString(Constant.getMysqlDateFormString());
            string ed= endTime.ToLocalTime().ToString(Constant.getMysqlDateFormString());
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                string sql= "SELECT SUM(onlineTime) FROM `online_time_daily` " +
                    "LEFT JOIN device ON device.id=online_time_daily.device " +
                    "WHERE online_time_daily.date>=@sd AND online_time_daily.date<ed " +
                    "AND device.hardwareDeviceID=@did";
                var query = connection.Query<double>(sql, new
                {
                    sd = sd,
                    ed = ed,
                    did = deviceId
                }).FirstOrDefault();
                return query;
            }
        }
        /*获取总时长*/
        public double GetTotalMinutesOnline(string deviceId)
        {
            using (var connection = new MySqlConnection(_databaseConStr.MySQL))
            {
                string sql = "SELECT SUM(onlineTime) FROM `online_time_daily` " +
                    "JOIN device ON device.id=online_time_daily.device AND device.hardwareDeviceID=@did";
                var query = connection.Query<double>(sql, new
                {
                    did = deviceId
                }).FirstOrDefault();
                return query;
            }
        }
    }
}