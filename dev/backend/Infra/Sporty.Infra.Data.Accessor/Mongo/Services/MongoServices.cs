using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using Sporty.Infra.Data.Accessor.Mongo.Interfaces;

namespace Sporty.Infra.Data.Accessor.Mongo.Services
{
    public class MongoServices : IMongoServices
    {
        private readonly List<Type> m_knownTypes = new List<Type>();

        /// <summary>
        /// Specifying Known Types for deserializing polymorphic classes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddKnownType<T>()
        {
            Type entityType = typeof(T);

            if(!m_knownTypes.Contains(entityType))
            {
                BsonClassMap.RegisterClassMap<T>();
                m_knownTypes.Add(entityType);
            }
        }
    }
}