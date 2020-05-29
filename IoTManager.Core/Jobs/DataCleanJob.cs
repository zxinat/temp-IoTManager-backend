using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Hangfire;
using MongoDB.Driver;
using IoTManager.Model.Configuration;
using IoTManager.Utility;
using Microsoft.Extensions.Options;

namespace IoTManager.Core.Jobs
{
    public class DataCleanJob
    {
        private readonly IMongoCollection<JobGraphModel> _jobGraphData;
        private readonly DatabaseConStr _databaseConStr;
        private readonly HangfireCollection _hangfireCollection;
        private readonly ILogger _logger;
        public DataCleanJob(ILogger<DataCleanJob> logger, IOptions<DatabaseConStr> databaseConStr,
            IOptions<HangfireCollection> hangfireCollection)
        {
            _logger = logger;
            _databaseConStr = databaseConStr.Value;
            _hangfireCollection = hangfireCollection.Value;
            var client = new MongoClient(_databaseConStr.MongoDB);
            var database = client.GetDatabase("iotmanager");
            _jobGraphData = database.GetCollection<JobGraphModel>(_hangfireCollection.Name);
        }
        public void DataCleanRun()
        {
            //数据定时清理Job
            _logger.LogInformation("DataCleanning...");
            DateTime lastMonth = DateTime.Now.AddMonths(-1);
            DateTime startTime = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            DateTime endTime = startTime.AddMonths(1);
            
        }

    }
}
