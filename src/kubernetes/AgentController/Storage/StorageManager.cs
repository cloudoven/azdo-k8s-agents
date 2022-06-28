using AgentController.AzDO;
using AgentController.Supports;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Azure.Data.Tables.Sas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgentController.Supports.ConfigUtils;

namespace AgentController.Storage
{
    public class StorageManager
    {
        private const string tableName = "azdojobmatrix";
        private readonly TableServiceClient serviceClient;
        private readonly InstrumentationClient instrumentation;

        public StorageManager(Config cfg, InstrumentationClient instrumentClient)
        {
            ArgumentNullException.ThrowIfNull(cfg);

            serviceClient = new TableServiceClient(
                new Uri($"https://{cfg.StorageAccountName}.table.core.windows.net/"),
                new TableSharedKeyCredential(cfg.StorageAccountName, cfg.StorageAccountKey));
            instrumentation = instrumentClient;
        }        

        public async Task<StorageManager> InitializeAsync()
        {
            await serviceClient.CreateTableIfNotExistsAsync(tableName);                        
            instrumentation.TrackEvent($"Storage table created/ensured.", new Dictionary<string, string> { { "tableName", tableName } });
            return this;
        }

        public async Task<bool> CheckJobAcknowledgementAsync(Job job)
        {
            ArgumentNullException.ThrowIfNull(job);

            var keys = StorageKeyPair.Get(job);
            var tableClient = this.serviceClient.GetTableClient(tableName);
            var query = $"PartitionKey eq '{keys.PartitionKey}' and RowKey eq '{keys.RowKey}'";
            var queryResultsFilter = tableClient.Query<TableEntity>(filter: query);
            await Task.CompletedTask;            
            return queryResultsFilter.Any();
        }

        public async Task UnregisterJobAsync(Job job)
        {
            ArgumentNullException.ThrowIfNull(job);

            var keys = StorageKeyPair.Get(job);
            var tableClient = this.serviceClient.GetTableClient(tableName);
            await tableClient.DeleteEntityAsync(keys.PartitionKey, keys.RowKey);
        }

        public async Task RegisterJobAsync(Job job)
        {
            ArgumentNullException.ThrowIfNull(job);

            var keys = StorageKeyPair.Get(job);
            var tableClient = this.serviceClient.GetTableClient(tableName);
            var entity = new TableEntity(keys.PartitionKey, keys.RowKey)
            {
                { "JobId", job.JobId },
                { "PlanId", job.PlanId },
                { "OrchestrationId", job.OrchestrationId },
                { "PoolId", job.PoolId },
                { "QueueTime", job.QueueTime },
            };
            await tableClient.AddEntityAsync(entity);
        }

        public class StorageKeyPair
        {
            public StorageKeyPair(string partitionKey, string rowKey)
            {
                PartitionKey = partitionKey;
                RowKey = rowKey;
            }

            public string PartitionKey { get; init; }
            public string RowKey { get; init; }

            public static StorageKeyPair Get(Job job)
            {
                return StorageKeyPair.Get(job.QueueTime, job.JobId);
            }

            public static StorageKeyPair Get(DateTimeOffset when, string jobId)
            {   
                return new StorageKeyPair($"{when.Year}-{when.Month}-{when.Day}", jobId);
            }
        }
    }
}
