using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Sporty.Infra.Data.Accessor.Mongo.Interfaces;
using Sporty.Infra.Data.Accessor.Mongo.Repository;

namespace Sporty.Services.Groups.DTO.Model
{
    public class Group : Document, IUpdateable<Group>
    {
        public string Name { get; set; }

        [BsonElement("Members", Order = 0)]
        public List<Guid> Members { get; set; } = new List<Guid>();

        public Group Update(Group update)
        {
            Name = update.Name;
            
            return this;
        }

        public void AddMember(Guid id)
        {
            Members.Add(id);
        }

        public bool ContainsMember(Guid id)
        {
            return Members.Contains(id);
        }

        public bool RemoveMember(Guid id)
        {
            return Members.Remove(id);
        }
    }
}
