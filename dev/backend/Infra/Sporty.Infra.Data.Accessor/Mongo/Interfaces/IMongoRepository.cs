using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sporty.Infra.Data.Accessor.Mongo.Interfaces
{
    public interface IMongoRepository<TDocument, TIdentifier>
        where TDocument : IDocument<TIdentifier>
    {
        /// <summary>
        /// Creates the collection for the repository.
        /// </summary>
        /// <param name="collectionName">The collection name</param>
        void Init(string collectionName);

        /// <summary>
        /// Adds the new entities to the repository.
        /// </summary>
        /// <param name="entities">The entities of type <typeparamref name="TDocument" /></param>
        /// <typeparam name="TDocument">The type of the entity.</typeparam>
        Task AddAsync(IEnumerable<TDocument> entities);

        /// <summary>
        /// Adds the new entities to the repository.
        /// </summary>
        /// <param name="entities">The entities of type <typeparamref name="TDocument" /></param>
        /// <typeparam name="TDocument">The type of the entity.</typeparam>
        void Add(IEnumerable<TDocument> entities);

        /// <summary>
        /// Adds an entity into the repository
        /// </summary>
        /// <param name="entity">Entity to insert</param>
        /// <returns>True if the insert has been successful otherwise false</returns>
        Task AddAsync(TDocument entity);

        /// <summary>
        /// Adds an entity into the repository
        /// </summary>
        /// <param name="entity">Entity to insert</param>
        /// <returns>True if the insert has been successful otherwise false</returns>
        void Add(TDocument entity);

        /// <summary>
        /// Adds an item to an entity inner collection that is already in the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns>The modified count</returns>
        Task<long> AddToCollectionFieldAsync<TItem>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem value);

        /// <summary>
        /// Adds an item to an entity inner collection that is already in the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns>The modified count</returns>
        long AddToCollectionField<TItem>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem value);

        /// <summary>
        /// Removes an entity from the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <returns>The deleted count</returns>
        Task<long> DeleteAsync(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Removes an entity from the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <returns>The deleted count</returns>
        long Delete(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Searches for a list of entities that match a specified predicate
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <returns>List of entities found</returns>
        Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Searches for a list of entities that match a specified predicate
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <returns>List of entities found</returns>
        IEnumerable<TDocument> Find(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Searches for a list of entities that match a specified predicate and returns the first
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <returns>The found entity</returns>
        Task<TDocument> FirstAsync(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Searches for a list of entities that match a specified predicate and returns the first
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <returns>The found entity</returns>
        TDocument First(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Searches for a list of entities that match a specified predicate and returns the first
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <returns>The found entity or null if not found</returns>
        Task<TDocument> FirstOrDefaultAsync(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Searches for a list of entities that match a specified predicate and returns the first
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <returns>The found entity or null if not found</returns>
        TDocument FirstOrDefault(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Searches for a list of entities fields that match a specified predicate
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <returns>List of entities found</returns>
        Task<IEnumerable<TField>> FindFieldAsync<TField>(
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, TField>> field);

        /// <summary>
        /// Searches for a list of entities fields that match a specified predicate
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <returns>List of entities found</returns>
        IEnumerable<TField> FindField<TField>(
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, TField>> field);

        /// <summary>
        /// Searches for a list of entities fields that match a specified predicate and returns the first
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <returns>List of entities found</returns>
        Task<TField> GetFieldAsync<TField>(
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, TField>> field);

        /// <summary>
        /// Searches for a list of entities fields that match a specified predicate and returns the first
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <returns>List of entities found</returns>
        TField GetField<TField>(
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, TField>> field);

        /// <summary>
        /// Searches for a list of entities fields that match a specified predicates
        /// </summary>
        /// <param name="documentFilter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="collectionFilter">Predicate to use when searching for collection entities.</param>
        /// <returns>List of entities found</returns>
        Task<IEnumerable<TItem>> FindCollectionItemAsync<TItem>(
            Expression<Func<TDocument, bool>> documentFilter,
            Expression<Func<TDocument, IEnumerable<TItem>>> field,
            Expression<Func<TItem, bool>> collectionFilter);

        /// <summary>
        /// Searches for a list of entities fields that match a specified predicates
        /// </summary>
        /// <param name="documentFilter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="collectionFilter">Predicate to use when searching for collection entities.</param>
        /// <returns>List of entities found</returns>
        IEnumerable<TItem> FindCollectionItem<TItem>(
            Expression<Func<TDocument, bool>> documentFilter,
            Expression<Func<TDocument, IEnumerable<TItem>>> field,
            Expression<Func<TItem, bool>> collectionFilter);

        /// <summary>
        /// Searches for a list of entities fields that match a specified predicates and returns the first
        /// </summary>
        /// <param name="documentFilter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="collectionFilter">Predicate to use when searching for collection entities.</param>
        /// <returns>First entity found</returns>
        Task<TItem> GetCollectionItemAsync<TItem>(
            Expression<Func<TDocument, bool>> documentFilter,
            Expression<Func<TDocument, IEnumerable<TItem>>> field,
            Expression<Func<TItem, bool>> collectionFilter);

        /// <summary>
        /// Searches for a list of entities fields that match a specified predicates and returns the first
        /// </summary>
        /// <param name="documentFilter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="collectionFilter">Predicate to use when searching for collection entities.</param>
        /// <returns>First entity found</returns>
        TItem GetCollectionItem<TItem>(
            Expression<Func<TDocument, bool>> documentFilter,
            Expression<Func<TDocument, IEnumerable<TItem>>> field,
            Expression<Func<TItem, bool>> collectionFilter);

        /// <summary>
        /// Updates an item from an entity inner collection that is already in the repository
        /// </summary>
        /// <param name="documentFilter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="collectionFilter">Predicate to use when searching for collection entities.</param>
        /// <param name="value">The value.</param>
        /// <returns>The modified count</returns>
        Task<long> UpdateCollectionItemAsync<TItem>(
            Expression<Func<TDocument, bool>> documentFilter,
            Expression<Func<TDocument, IEnumerable<TItem>>> field,
            Expression<Func<TItem, bool>> collectionFilter,
            TItem value);

        /// <summary>
        /// Updates an item from an entity inner collection that is already in the repository
        /// </summary>
        /// <param name="documentFilter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="collectionFilter">Predicate to use when searching for collection entities.</param>
        /// <param name="value">The value.</param>
        /// <returns>The modified count</returns>
        long UpdateCollectionItem<TItem>(
            Expression<Func<TDocument, bool>> documentFilter,
            Expression<Func<TDocument, IEnumerable<TItem>>> field,
            Expression<Func<TItem, bool>> collectionFilter,
            TItem value);

        /// <summary>
        /// Retrieves a specific number of entities from the repository from a specific position
        /// </summary>
        /// <returns>List of entities</returns>
        Task<IEnumerable<TDocument>> GetAsync(int count, int start);

        /// <summary>
        /// Retrieves a specific number of entities from the repository from a specific position
        /// </summary>
        /// <returns>List of entities</returns>
        //IEnumerable<TDocument> Get(int count, int start);

        /// <summary>
        /// Retrieves all the entities from the repository
        /// </summary>
        /// <returns>List of entities</returns>
        Task<IEnumerable<TDocument>> GetAsync();

        /// <summary>
        /// Retrieves all the entities from the repository
        /// </summary>
        /// <returns>List of entities</returns>
        IEnumerable<TDocument> Get();

        /// <summary>
        /// Removes an item from an entity inner collection that is already in the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns>The modified count</returns>
        Task<long> RemoveFromCollectionFieldAsync<TItem>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem value);

        /// <summary>
        /// Removes an item from an entity inner collection that is already in the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TItem">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns>The modified count</returns>
        long RemoveFromCollectionField<TItem>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem value);

        /// <summary>
        /// Update an entity that is already in the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <param name="updatedEntity">The updated entity</param>
        /// <returns>The modified count</returns>
        Task<long> UpdateAsync(Expression<Func<TDocument, bool>> filter, TDocument updatedEntity);

        /// <summary>
        /// Update an entity that is already in the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <param name="updatedEntity">The updated entity</param>
        /// <returns>The modified count</returns>
        long Update(Expression<Func<TDocument, bool>> filter, TDocument updatedEntity);

        /// <summary>
        /// Update an entity field that is already in the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns>The modified count</returns>
        Task<long> UpdateFieldAsync<TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field, TField value);

        /// <summary>
        /// Update an entity field that is already in the repository
        /// </summary>
        /// <param name="filter">Predicate to use when searching for entities</param>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns>The modified count</returns>
        long UpdateField<TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field, TField value);

        /// <summary>
        /// Clears the collection
        /// </summary>
        /// <returns>The deleted count</returns>
        Task<long> ClearAsync();

        /// <summary>
        /// Clears the collection
        /// </summary>
        /// <returns>The deleted count</returns>
        long Clear();

        /// <summary>
        /// Drops the collection
        /// </summary>
        Task DropAsync();

        /// <summary>
        /// Drops the collection
        /// </summary>
        void Drop();
    }
}
