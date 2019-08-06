using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IoTManager.MonitorService.Utility
{
    public interface IAppSetting
    {
        string SqlDBConnectionString { get; }
        string MongoDBConnectionString { get; }
        string MongoDBName { get; }
        string MongoCollectionName { get; }
        string StorageConnectionString { get; }
        string LogTableName { get; }
    }

    public sealed class AppSetting : IAppSetting
    {
        private readonly IConfigurationRoot _configuration;
        public AppSetting()
        {
            this._configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public string SqlDBConnectionString => this._configuration.GetValue<string>("SQLDBConnectionString");

        public string MongoDBConnectionString => this._configuration.GetValue<string>("MongoDBConnectionString");

        public string MongoDBName => this._configuration.GetValue<string>("MongoDBName");

        public string MongoCollectionName => this._configuration.GetValue<string>("MongoCollectionName");

        public string StorageConnectionString => this._configuration.GetValue<string>("StorageConnectionString");

        public string LogTableName => this._configuration.GetValue<string>("LogTableName");
    }
}
