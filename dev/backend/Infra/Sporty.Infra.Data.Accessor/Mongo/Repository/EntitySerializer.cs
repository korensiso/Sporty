using System;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Sporty.Infra.Data.Accessor.Mongo.Repository
{
    public class EntitySerializer<T> : SerializerBase<T>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            BsonSerializer.Serialize(context.Writer, args);
        }

        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            BsonDocument bdoc = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
            T obj = (T)Activator.CreateInstance(typeof(T), BindingFlags.CreateInstance, bdoc);
            return obj;
        }
    }
}
