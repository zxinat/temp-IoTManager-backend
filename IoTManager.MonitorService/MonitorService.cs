using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using IoTManager.Utility.Helpers;
using IoTManager.Utility.Extensions;
using System.Data;
using System.Threading.Tasks;
using IoTManager.Model.Contracts;
using Microsoft.Extensions.Configuration;
using IoTManager.MonitorService.Utility;
using System;
using IoTManager.Model;
using System.Collections.Generic;
using IoTManager.MonitorService.Proxy;

namespace IoTManager.MonitorService
{
    public static class MonitorService
    {
        [FunctionName("MonitorService")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            IAppSetting appSetting = new AppSetting();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            IStorageProxy storageProxy = new StorageProxy(appSetting.StorageConnectionString);
            await storageProxy.WriteTable(appSetting.LogTableName, requestBody);
            if (string.IsNullOrEmpty(requestBody))
            {
                return new StatusCodeResult(404);
            }
            List<MonitorDataModel> monitorDatas = new List<MonitorDataModel>();
            try
            {
                var singleMonitorDatas = requestBody.ConvertToObj<List<SingleMonitorDataContract>>();
                foreach (var singleMonitorData in singleMonitorDatas)
                {
                    foreach (var data in singleMonitorData.Data.IData)
                    {
                        var device = singleMonitorData.Data.DName.Split(',');
                        MonitorDataModel model = new MonitorDataModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            GatewayId = singleMonitorData.GSN,
                            DeviceId = singleMonitorData.Data.DId,
                            DeviceName = device[0],
                            DeviceType = device.Length == 2 ? device[1] : string.Empty,
                            MonitorId = data.IId,
                            MonitorName = data.IName,
                            MonitorType = data.IType,
                            Unit = data.IUnit,
                            Value = data.IValue,
                            IsScam = false,
                            Timestamp = DateTime.Now
                        };
                        monitorDatas.Add(model);
                    }
                }
            }
            catch (Exception)
            {
                var multpleMonitorDatas = requestBody.ConvertToObj<List<MultipleMonitorDataContract>>();
                foreach (var multpleMonitorData in multpleMonitorDatas)
                {
                    foreach (var monitorData in multpleMonitorData.Data)
                    {
                        var device = monitorData.DName.Split(',');
                        foreach (var data in monitorData.IData)
                        {
                            MonitorDataModel model = new MonitorDataModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                GatewayId = multpleMonitorData.GSN,
                                DeviceId = monitorData.DId,
                                DeviceName = device[0],
                                DeviceType = device.Length == 2 ? device[1] : string.Empty,
                                MonitorId = data.IId,
                                MonitorName = data.IName,
                                MonitorType = data.IType,
                                Unit = data.IUnit,
                                Value = data.IValue,
                                IsScam = false,
                                Timestamp = DateTime.Now
                            };
                            monitorDatas.Add(model);
                        }
                    }
                }
            }
            MongoHelper mongoHelper = new MongoHelper(appSetting.MongoDBConnectionString, appSetting.MongoDBName);
            foreach (var data in monitorDatas)
            {
                mongoHelper.InsertOne<MonitorDataModel>(appSetting.MongoCollectionName, data);
            }

            //TODO:insert monitor data to database
            //using (SqlHelper helper = new SqlHelper(connectionString))
            //{
            //    string sql = "insert into ....";
            //    helper.ExecuteNonQuery(sql, CommandType.Text);
            //}
            return new OkObjectResult($"Hello, IoTManager");
        }
    }
}
