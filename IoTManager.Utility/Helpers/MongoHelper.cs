using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace IoTManager.Utility.Helpers
{
    public sealed class MongoHelper
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;

        public MongoHelper(string connectionString, string databaseName)
        {
            this._mongoClient = new MongoClient(connectionString);
            if (this.DatabaseIsExist(databaseName))
            {
                var database = this._mongoClient.GetDatabase(databaseName);
                this._mongoDatabase = database;
            }
            else
            {
                throw new Exception($"the databse({databaseName}) is not exist");
            }
        }

        /// <summary>
        /// 判断数据库是否存在
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        private bool DatabaseIsExist(string databaseName)
        {
            var databaseNames = this._mongoClient.ListDatabaseNames().ToList();
            return databaseNames.Contains(databaseName);
        }

        /// <summary>
        /// 创建集合如果集合不存在
        /// </summary>
        /// <param name="collectionName"></param>
        private IMongoCollection<TCollection> CreateCollectionIfNotExist<TCollection>(string collectionName)
        {
            var collections = this._mongoDatabase.ListCollectionNames().ToList();
            if (collections.Contains(collectionName))
            {
                return this._mongoDatabase.GetCollection<TCollection>(collectionName);
            }
            else
            {
                this._mongoDatabase.CreateCollection(collectionName);
                return this._mongoDatabase.GetCollection<TCollection>(collectionName);
            }
        }

        private List<UpdateDefinition<TDocument>> BuildUpdateDefinition<TDocument>(TDocument document)
        {
            List<UpdateDefinition<TDocument>> updateDefinitions = new List<UpdateDefinition<TDocument>>();
            var properties = typeof(TDocument).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var updateDefinition = Builders<TDocument>.Update.Set(property.Name, property.GetValue(document));
                updateDefinitions.Add(updateDefinition);
            }
            return updateDefinitions;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="document"></param>
        public void InsertOne<TCollection>(string collectionName, TCollection document)
        {
            var mongoCollection = this.CreateCollectionIfNotExist<TCollection>(collectionName);
            mongoCollection.InsertOne(document);
        }

        /// <summary>
        /// 查找符合条件的数据
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<TCollection> Find<TCollection>(string collectionName, Expression<Func<TCollection, bool>> filter)
        {
            var mongoCollection = this.CreateCollectionIfNotExist<TCollection>(collectionName);
            return mongoCollection.Find(filter).ToList();
        }

        public void UpdateMany<TCollection>(string collectionName, Expression<Func<TCollection, bool>> filter, TCollection document)
        {
            var mongoCollection = this.CreateCollectionIfNotExist<TCollection>(collectionName);
            var updateDefinitions = this.BuildUpdateDefinition<TCollection>(document);
            mongoCollection.UpdateMany(filter, Builders<TCollection>.Update.Combine(updateDefinitions));
        }
    }
}
