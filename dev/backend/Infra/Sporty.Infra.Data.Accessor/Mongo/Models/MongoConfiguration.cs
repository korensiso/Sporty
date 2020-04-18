using Sporty.Infra.Data.Accessor.Mongo.Interfaces;

namespace Sporty.Infra.Data.Accessor.Mongo.Models
{
    public class MongoConfiguration : IMongoConfiguration
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public override string ToString()
        {
            return $"ConnectionString={ConnectionString}, DatabaseName={DatabaseName}";
        }
    }
}