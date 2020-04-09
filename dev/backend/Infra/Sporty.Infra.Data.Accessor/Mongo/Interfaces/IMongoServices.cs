namespace Sporty.Infra.Data.Accessor.Mongo.Interfaces
{
    public interface IMongoServices
    {
        /// <summary>
        /// Specifying Known Types for deserializing polymorphic classes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void AddKnownType<T>();
    }
}
