using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility;

namespace IoTManager.Dao
{
    public sealed class ThresholdDao: IThresholdDao
    {
        public Dictionary<String, Tuple<String, int>> GetByDeviceId(String deviceId)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                List<ThresholdModel> thresholdModels = connection.Query<ThresholdModel>(
                    "select * from threshold where deviceId=@did", new
                    {
                        did = deviceId
                    }).ToList();
                
                Dictionary<String, Tuple<String, int>> result = new Dictionary<string, Tuple<string, int>>();
                foreach (ThresholdModel t in thresholdModels)
                {
                    result.Add(t.IndexId, new Tuple<string, int>(t.Operator, t.ThresholdValue));
                }

                return result;
            }
        }

        public String Create(ThresholdModel thresholdModel)
        {
            using (var connection = new SqlConnection(Constant.getDatabaseConnectionString()))
            {
                int rows = connection.Execute(
                    "insert into threshold(indexId, deviceId, operator, thresholdValue, ruleName, description) values(@iid, @did, @o, @tv, @rn, @d)", new
                    {
                        iid = thresholdModel.IndexId,
                        did = thresholdModel.DeviceId,
                        o = thresholdModel.Operator,
                        tv = thresholdModel.ThresholdValue,
                        rn = thresholdModel.RuleName,
                        d = thresholdModel.Description
                    });
                return rows == 1 ? "success" : "error";
            }
        }
    }
}