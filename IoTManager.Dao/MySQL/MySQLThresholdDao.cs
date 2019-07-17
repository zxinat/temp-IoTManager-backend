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
    public sealed class MySQLThresholdDao : IThresholdDao
    {
        public List<ThresholdModel> GetByDeviceId(String deviceId)
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                List<ThresholdModel> thresholdModels = connection.Query<ThresholdModel>(
                    "select * from threshold where deviceId=@did", new
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

        public List<ThresholdModel> Get()
        {
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                return connection.Query<ThresholdModel>("select threshold.id, fieldName indexId, deviceId, operator, thresholdValue, threshold.createTime, threshold.updateTime, ruleName, description, severity.severityName severity from threshold inner join field on indexId=fieldId inner join severity on threshold.severity=severity.id").ToList();
            }
        }
    }
}