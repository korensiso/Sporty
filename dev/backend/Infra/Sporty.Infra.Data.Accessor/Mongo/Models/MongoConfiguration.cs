using Sporty.Infra.Data.Accessor.Mongo.Interfaces;

namespace Sporty.Infra.Data.Accessor.Mongo.Models
{
    public class MongoConfiguration : IMongoConfiguration
    {
        public MongoConfiguration(string connectionString, string databaseName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }

        public string ConnectionString { get; }
        public string DatabaseName { get; }
    }
}