using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Sporty.Infra.Data.Accessor.Mongo.Interfaces;
using Sporty.Infra.Data.Accessor.Mongo.Repository;

namespace Sporty.Common.Dto.Group.Model
{
    public class Group : Document, IUpdateable<Group>
    {
        public string Name { get; set; }

        [BsonElement("Users", Order = 0)]
        public List<Guid> Users { get; } = new List<Guid>();

        public Group Update(Group update)
        {
            Name = update.Name;
            
            return this;
        }

        public void AddUser(Guid id)
        {
            Users.Add(id);
        }

        public bool RemoveUser(Guid id)
        {
            return Users.Remove(id);
        }
    }
}
