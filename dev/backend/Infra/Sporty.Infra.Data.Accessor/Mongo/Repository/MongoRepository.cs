using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Sporty.Infra.Data.Accessor.Mongo.Interfaces;

namespace Sporty.Infra.Data.Accessor.Mongo.Repository
{
    public class MongoRepository<TDocument, TIdentifier> : IMongoRepository<TDocument, TIdentifier>
        where TDocument : IDocument<TIdentifier>, IUpdateable<TDocument>
    {
        private readonly IMongoDatabase _database;
        private IMongoCollection<TDocument> _collection;
        private string _collectionName;

        public MongoRepository(IMongoConfiguration mongoConfiguration)
        {
            MongoClient client = new MongoClient(mongoConfiguration.ConnectionString);
            _database = client.GetDatabase(mongoConfiguration.DatabaseName);
        }

        /// <inheritdoc />
        public void Init(string collectionName)
        {
            bool collectionExists = IsCollectionExists(collectionName);
            if (!collectionExists)
            {
                _database.CreateCollection(collectionName);
            }

            _collection = _database.GetCollection<TDocument>(collectionName);
            _collectionName = collectionName;
        }

        private bool IsCollectionExists(string collectionName)
        {
            BsonDocument filter = new BsonDocument("name", collectionName);
            IAsyncCursor<BsonDocument> collections = _database.ListCollections(new ListCollectionsOptions {Filter = filter});
            return collections.Any();
        }
        
        /// <inheritdoc />
        public async Task AddAsync(TDocument entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        /// <inheritdoc />
        public void Add(TDocument entity)
        {
            AsyncHelper.RunSync(() => AddAsync(entity));
        }

        /// <inheritdoc />
        public async Task AddAsync(IEnumerable<TDocument> entities)
        {
            IEnumerable<TDocument> documents = entities as IList<TDocument> ?? entities.ToList();
            await _collection.InsertManyAsync(documents);
        }

        /// <inheritdoc />
        public void Add(IEnumerable<TDocument> entities)
        {
            AsyncHelper.RunSync(() => AddAsync(entities));
        }

        /// <inheritdoc />
        public async Task<long> UpdateAsync(Expression<Func<TDocument, bool>> filter, TDocument updatedEntity)
        {
            ReplaceOneResult replaceResult =
                await _collection.ReplaceOneAsync(filter, updatedEntity, new ReplaceOptions { IsUpsert = true });

            return replaceResult.ModifiedCount;
        }

        /// <inheritdoc />
        public long Update(Expression<Func<TDocument, bool>> filter, TDocument updatedEntity)
        {
            return AsyncHelper.RunSync(() => UpdateAsync(filter, updatedEntity));
        }

        /// <inheritdoc />
        public async Task<long> UpdateFieldAsync<TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field, TField value)
        {
            UpdateDefinition<TDocument> updateDefinition = Builders<TDocument>.Update.Set(field, value);
            UpdateResult updateResult = await _collection.UpdateOneAsync(filter, updateDefinition);

            return updateResult.ModifiedCount;
        }

        /// <inheritdoc />
        public long UpdateField<TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field, TField value)
        {
            return AsyncHelper.RunSync(() => UpdateFieldAsync(filter, field, value));
        }

        /// <inheritdoc />
        public async Task<long> AddToCollectionFieldAsync<TItem>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem value)
        {
            UpdateDefinition<TDocument> updateDefinition = Builders<TDocument>.Update.Push(field, value);
            UpdateResult updateResult = await _collection.UpdateOneAsync(filter, updateDefinition);

            return updateResult.ModifiedCount;
        }

        /// <inheritdoc />
        public long AddToCollectionField<TItem>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem value)
        {
            return AsyncHelper.RunSync(() => AddToCollectionFieldAsync(filter, field, value));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TDocument>> GetAsync(int count, int start = 0)
        {
            var findOptions = new FindOptions<TDocument> { Limit = count, Skip = start};
            IAsyncCursor<TDocument> cursor = await _collection.FindAsync(pred => true, findOptions);

            return await cursor.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TDocument>> GetAsync()
        {
            IAsyncCursor<TDocument> cursor = await _collection.FindAsync(pred => true);

            return await cursor.ToListAsync();
        }

        /// <inheritdoc />
        public IEnumerable<TDocument> Get()
        {
            return AsyncHelper.RunSync(GetAsync);
        }

        /// <inheritdoc />
        public async Task<long> RemoveFromCollectionFieldAsync<TItem>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem value)
        {
            UpdateDefinition<TDocument> updateDefinition = Builders<TDocument>.Update.Pull(field, value);
            UpdateResult updateResult = await _collection.UpdateOneAsync(filter, updateDefinition);

            return updateResult.ModifiedCount;
        }

        /// <inheritdoc />
        public long RemoveFromCollectionField<TItem>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem value)
        {
            return AsyncHelper.RunSync(() => RemoveFromCollectionFieldAsync(filter, field, value));
        }

        /// <inheritdoc />
        public async Task<long> DeleteAsync(Expression<Func<TDocument, bool>> filter)
        {
            DeleteResult deleteResult = await _collection.DeleteManyAsync(filter);

            return deleteResult.DeletedCount;
        }

        /// <inheritdoc />
        public long Delete(Expression<Func<TDocument, bool>> filter)
        {
            return AsyncHelper.RunSync(() => DeleteAsync(filter));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter)
        {
            IMongoQueryable<TDocument> documents = _collection.AsQueryable();
            IMongoQueryable<TDocument> filterDocuments = documents.Where(filter);

            return await filterDocuments.ToListAsync();
        }

        /// <inheritdoc />
        public IEnumerable<TDocument> Find(Expression<Func<TDocument, bool>> filter)
        {
            return AsyncHelper.RunSync(() => FindAsync(filter));
        }

        /// <inheritdoc />
        public async Task<TDocument> FirstAsync(Expression<Func<TDocument, bool>> filter)
        {
            IAsyncCursor<TDocument> cursor = await _collection.FindAsync(filter);

            return await cursor.FirstAsync();
        }

        /// <inheritdoc />
        public TDocument First(Expression<Func<TDocument, bool>> filter)
        {
            return AsyncHelper.RunSync(() => FirstAsync(filter));
        }

        /// <inheritdoc />
        public async Task<TDocument> FirstOrDefaultAsync(Expression<Func<TDocument, bool>> filter)
        {
            IAsyncCursor<TDocument> cursor = await _collection.FindAsync(filter);
            return await cursor.FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public TDocument FirstOrDefault(Expression<Func<TDocument, bool>> filter)
        {
            return AsyncHelper.RunSync(() => FirstOrDefaultAsync(filter));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TField>> FindFieldAsync<TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field)
        {
            return await _collection.Find(filter).Project(new ProjectionDefinitionBuilder<TDocument>().Expression(field))
                .ToListAsync();
        }

        /// <inheritdoc />
        public IEnumerable<TField> FindField<TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field)
        {
            return AsyncHelper.RunSync(() => FindFieldAsync(filter, field));
        }

        /// <inheritdoc />
        public async Task<TField> GetFieldAsync<TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field)
        {
            return await _collection.Find(filter).Project(new ProjectionDefinitionBuilder<TDocument>().Expression(field))
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public TField GetField<TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field)
        {
            return AsyncHelper.RunSync(() => GetFieldAsync(filter, field));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TItem>> FindCollectionItemAsync<TItem>(Expression<Func<TDocument, bool>> documentFilter, Expression<Func<TDocument, IEnumerable<TItem>>> field, Expression<Func<TItem, bool>> collectionFilter)
        {
            IMongoQueryable<TDocument> documents = _collection.AsQueryable();
            List<TDocument> filterDocuments = await documents.Where(documentFilter).ToListAsync();
            IEnumerable<TItem> items = filterDocuments.SelectMany(field.Compile());
            IEnumerable<TItem> filteredItems = items.Where(collectionFilter.Compile());

            return filteredItems;
        }

        /// <inheritdoc />
        public IEnumerable<TItem> FindCollectionItem<TItem>(Expression<Func<TDocument, bool>> documentFilter, Expression<Func<TDocument, IEnumerable<TItem>>> field, Expression<Func<TItem, bool>> collectionFilter)
        {
            return AsyncHelper.RunSync(() => FindCollectionItemAsync(documentFilter, field, collectionFilter));
        }

        /// <inheritdoc />
        public async Task<TItem> GetCollectionItemAsync<TItem>(Expression<Func<TDocument, bool>> documentFilter, Expression<Func<TDocument, IEnumerable<TItem>>> field, Expression<Func<TItem, bool>> collectionFilter)
        {
            IEnumerable<TItem> items = await FindCollectionItemAsync(documentFilter, field, collectionFilter);
            return items.FirstOrDefault();
        }

        /// <inheritdoc />
        public TItem GetCollectionItem<TItem>(Expression<Func<TDocument, bool>> documentFilter, Expression<Func<TDocument, IEnumerable<TItem>>> field, Expression<Func<TItem, bool>> collectionFilter)
        {
            return AsyncHelper.RunSync(() => GetCollectionItemAsync(documentFilter, field, collectionFilter));
        }

        /// <inheritdoc />
        public async Task<long> UpdateCollectionItemAsync<TItem>(Expression<Func<TDocument, bool>> documentFilter, Expression<Func<TDocument, IEnumerable<TItem>>> field, Expression<Func<TItem, bool>> collectionFilter, TItem value)
        {
            TItem oldItem = await GetCollectionItemAsync(documentFilter, field, collectionFilter);
            long modifiedCount = await RemoveFromCollectionFieldAsync(documentFilter, field, oldItem);
            if (modifiedCount > 0)
            {
                modifiedCount = await AddToCollectionFieldAsync(documentFilter, field, value);
            }

            return modifiedCount;
        }

        /// <inheritdoc />
        public long UpdateCollectionItem<TItem>(Expression<Func<TDocument, bool>> documentFilter, Expression<Func<TDocument, IEnumerable<TItem>>> field, Expression<Func<TItem, bool>> collectionFilter, TItem value)
        {
            return AsyncHelper.RunSync(() => UpdateCollectionItemAsync(documentFilter, field, collectionFilter, value));
        }

        /// <inheritdoc />
        public async Task<long> ClearAsync()
        {
            DeleteResult deleteResult = await _collection.DeleteManyAsync(pred => true);
            return deleteResult.DeletedCount;
        }

        /// <inheritdoc />
        public long Clear()
        {
            return AsyncHelper.RunSync(ClearAsync);
        }

        /// <inheritdoc />
        public async Task DropAsync()
        {
            await _database.DropCollectionAsync(_collectionName);
        }

        /// <inheritdoc />
        public void Drop()
        {
            AsyncHelper.RunSync(DropAsync);
        }
    }
}