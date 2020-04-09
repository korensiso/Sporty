namespace Sporty.Infra.Data.Accessor.Mongo.Interfaces
{
    public interface IMongoConfiguration
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
    }
}