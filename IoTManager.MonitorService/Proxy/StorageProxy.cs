using IoTManager.MonitorService.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IoTManager.MonitorService.Proxy
{
    public interface IStorageProxy
    {
        Task WriteTable(string tableName, string message);
    }

    public sealed class StorageProxy : IStorageProxy
    {
        private readonly CloudStorageAccount _cloudStoraegAccount;

        public StorageProxy(string connectionString)
        {
            this._cloudStoraegAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task WriteTable(string tableName, string message)
        {
            CloudTableClient tableClient = this._cloudStoraegAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            string partitionKey = DateTime.Now.ToString("yyyyMM");
            string rowKey = DateTime.Now.ToString("ddHHmmssffff");
            TableOperation operation = TableOperation.Insert(new LogEntity { PartitionKey = partitionKey, RowKey = rowKey, Message = message, Timestamp = DateTimeOffset.Now, ETag = string.Empty });
            await table.ExecuteAsync(operation);
        }
    }
}
