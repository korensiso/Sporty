using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sporty.Infra.Data.Accessor.Mongo.Interfaces;

namespace Sporty.Infra.Data.Accessor.Mongo.Repository
{
    [BsonSerializer(typeof(EntitySerializer<>))]
    public abstract class Document : IDocument<Guid>
    {
        [BsonId]
        [BsonElement("_id", Order = 0)]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string DocumentId { get; set; }

        public Guid Identifier { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
