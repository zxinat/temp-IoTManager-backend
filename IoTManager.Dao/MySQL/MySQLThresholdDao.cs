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

        public long GetThresholdNumber(String searchType, List<DeviceModel> devices)
        {
            long number = 0;
            List<ThresholdModel> allThreshold = new List<ThresholdModel>();
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                allThreshold = connection.Query<ThresholdModel>(
                    "select * from threshold").ToList();
//                allThreshold = connection.Query<ThresholdModel>("select threshold.id, fieldName indexId, deviceId, operator, thresholdValue, threshold.createTime, threshold.updateTime, ruleName, description, severity.severityName severity from threshold inner join field on indexId=fieldId inner join severity on threshold.severity=severity.id").ToList();
            }
            if (searchType == "search")
            {
                foreach (var device in devices )
                {
                    var query = allThreshold.AsQueryable()
                        .Where(dd => dd.DeviceId == device.HardwareDeviceId)
                        .ToList();
                    number += query.Count;
                }
            }
            else
            {
                number = allThreshold.Count;
            }

            return number;
        }
        public List<ThresholdModel> Get(String searchType, List<DeviceModel>devices, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc")
        {
            List<ThresholdModel> allThreshold = new List<ThresholdModel>();
            using (var connection = new MySqlConnection(Constant.getDatabaseConnectionString()))
            {
                allThreshold = connection.Query<ThresholdModel>(
                    "select threshold.id, indexId, deviceId, operator, thresholdValue, threshold.createTime, threshold.updateTime, ruleName, description, severity.severityName severity from threshold inner join severity on threshold.severity=severity.id").ToList();
//                allThreshold = connection.Query<ThresholdModel>("select threshold.id, fieldName indexId, deviceId, operator, thresholdValue, threshold.createTime, threshold.updateTime, ruleName, description, severity.severityName severity from threshold inner join field on indexId=fieldId inner join severity on threshold.severity=severity.id").ToList();
            }
            List<ThresholdModel> selected = new List<ThresholdModel>();
            if (searchType == "search")
            {
                foreach (var device in devices )
                {
                    var query = allThreshold.AsQueryable()
                        .Where(dd => dd.DeviceId == device.HardwareDeviceId)
                        .ToList();
                    foreach (var q in query)
                    {
                        selected.Add(q);
                    }
                }
            }
            else
            {
                selected = allThreshold;
            }
            List<ThresholdModel> result = new List<ThresholdModel>();
            if (order != "no" && sortColumn != "no")
            {
                if (sortColumn == "DeviceId")
                {
                    if (order == "asc")
                    {
                        result = selected.AsQueryable()
                            .OrderBy(dd => dd.DeviceId)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = selected.AsQueryable()
                            .OrderByDescending(dd => dd.DeviceId)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "IndexId")
                {
                    if (order == "asc")
                    {
                        result = selected.AsQueryable()
                            .OrderBy(dd => dd.IndexId)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = selected.AsQueryable()
                            .OrderByDescending(dd => dd.IndexId)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "CreateTime")
                {
                    if (order == "asc")
                    {
                        result = selected.AsQueryable()
                            .OrderBy(dd => dd.CreateTime)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = selected.AsQueryable()
                            .OrderByDescending(dd => dd.CreateTime)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "UpdateTime")
                {
                    if (order == "asc")
                    {
                        result = selected.AsQueryable()
                            .OrderBy(dd => dd.UpdateTime)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = selected.AsQueryable()
                            .OrderByDescending(dd => dd.UpdateTime)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "Id")
                {
                    if (order == "asc")
                    {
                        result = selected.AsQueryable()
                            .OrderBy(dd => dd.Id)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = selected.AsQueryable()
                            .OrderByDescending(dd => dd.Id)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "RuleName")
                {
                    if (order == "asc")
                    {
                        result = selected.AsQueryable()
                            .OrderBy(dd => dd.RuleName)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = selected.AsQueryable()
                            .OrderByDescending(dd => dd.RuleName)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
                else if (sortColumn == "Severity")
                {
                    if (order == "asc")
                    {
                        result = selected.AsQueryable()
                            .OrderBy(dd => dd.Severity)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                    else if (order == "desc")
                    {
                        result = selected.AsQueryable()
                            .OrderByDescending(dd => dd.Severity)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
                    }
                }
            }

            return result;
        }
    }
}