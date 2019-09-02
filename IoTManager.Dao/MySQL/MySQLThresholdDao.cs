using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;

namespace IoTManager.Dao
{
    public sealed class MySQLThresholdDao : IThresholdDao
    {
        public List<ThresholdModel> GetByDeviceId(String deviceId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                List<ThresholdModel> thresholdModels = connection.Query<ThresholdModel>(
                    "select threshold.id, indexId, deviceId, operator, thresholdValue, threshold.createTime, threshold.updateTime, ruleName, description, severity.severityName severity from threshold inner join severity on threshold.severity=severity.id where deviceId=@did", new
                    {
                        did = deviceId
                    }).ToList();
                
                

                return thresholdModels;
            }
        }

        public String Create(ThresholdModel thresholdModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                SeverityModel severity = connection
                        .Query<SeverityModel>("select * from severity where severityName=@sn",
                            new { sn = thresholdModel.Severity }).FirstOrDefault();
                int rows = connection.Execute(
                    "insert into threshold(indexId, deviceId, operator, thresholdValue, ruleName, description, severity) values(@iid, @did, @o, @tv, @rn, @d, @sid)", new
                    {
                        iid = thresholdModel.IndexId,
                        did = thresholdModel.DeviceId,
                        o = thresholdModel.Operator,
                        tv = thresholdModel.ThresholdValue,
                        rn = thresholdModel.RuleName,
                        d = thresholdModel.Description,
                        sid = severity.Id
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public String Update(int id, ThresholdModel thresholdModel)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                SeverityModel severity = connection
                    .Query<SeverityModel>("select * from severity where severityName=@sn",
                        new { sn = thresholdModel.Severity }).FirstOrDefault();
                int rows = connection.Execute(
                    "update threshold set indexId=@iid, deviceId=@did, operator=@o, thresholdValue=@tv, ruleName=@rn, description=@d, severity=@sid where id=@i",
                    new
                    {
                        i = id,
                        iid = thresholdModel.IndexId,
                        did = thresholdModel.DeviceId,
                        o = thresholdModel.Operator,
                        tv = thresholdModel.ThresholdValue,
                        rn = thresholdModel.RuleName,
                        d = thresholdModel.Description,
                        sid = severity.Id
                    });
                return rows == 1 ? "success" : "error";
            }
        }

        public long GetThresholdNumber(String searchType, String deviceName = "all")
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                String s = "select count(*) number from threshold ";
                if (searchType == "search")
                {
                    if (deviceName != "all")
                    {
                        s += "where deviceId=@dn";
                    }
                    var result = connection.Query(s, new {dn = deviceName}).FirstOrDefault();
                    return result.number;
                }
                else
                {
                    var result = connection.Query(s).FirstOrDefault();
                    return result.number;
                }
            }
        }
        public List<ThresholdModel> Get(String searchType, String deviceName = "all", int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc")
        {
            String s =
                "select threshold.id, indexId, deviceId, operator, thresholdValue, threshold.createTime, threshold.updateTime, ruleName, description, severity.severityName severity from threshold inner join severity on threshold.severity=severity.id ";
            if (searchType == "search")
            {
                if (deviceName != "all")
                {
                    s += "where threshold.deviceId=@dn ";
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
                return connection.Query<ThresholdModel>(s, new {dn=deviceName}).ToList();
            }
        }

        public String Delete(int id)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute("delete from threshold where id=@i", new {i = id});
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
                    connection.Execute("delete from threshold where threshold.id=@tid", new
                    {
                        tid = i
                    });
                    rows = rows + 1;
                }

                return rows;
            }
        }

        public int GetDeviceAffiliateThreshold(String deviceId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                int result = connection.Query<int>("select count(*) number from threshold where deviceId=@did", new
                {
                    did = deviceId
                }).FirstOrDefault();
                return result;
            }
        }
    }
}