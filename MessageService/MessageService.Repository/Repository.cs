using System.Linq.Expressions;

using MongoDB.Bson;
using MongoDB.Driver;

using MessageService.Repository.Interface;
using MessageService.Model.MongoDB;
using MessageService.Repository.Interface.Pagination;

namespace MessageService.Repository;

public class Repository<T> : IRepository<T> where T : IDocument
{
    protected readonly IMongoCollection<T> _collection;

    public Repository(IMongoDbSettings settings)
    {
        var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
        _collection = database.GetCollection<T>(GetCollectionName(typeof(T)));
    }

    private protected string? GetCollectionName(Type documentType)
    {
        return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                typeof(BsonCollectionAttribute),
                true)
            .FirstOrDefault())?.CollectionName;
    }

    public IQueryable<T> AsQueryable()
    {
        return _collection.AsQueryable();
    }

    public Task DeleteByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
        return _collection.FindOneAndDeleteAsync(filter);
    }

    public async Task InsertManyAsync(ICollection<T> documents)
    {
        await _collection.InsertManyAsync(documents);
    }

    public Task InsertOneAsync(T document)
    {
        return _collection.InsertOneAsync(document);
    }

    public async Task ReplaceOneAsync(T document)
    {
        var filter = Builders<T>.Filter.Eq(doc => doc.Id, document.Id);
        await _collection.FindOneAndReplaceAsync(filter, document);
    }

    public Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).FirstOrDefaultAsync();
    }

    public Task<T> FindByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
        return _collection.Find(filter).SingleOrDefaultAsync();
    }

    public PagedList<T> FilterBy(Expression<Func<T, bool>> filterExpression, PaginationParams paginationParams)
    {
        return PagedList<T>.ToPagedList(_collection.Find(filterExpression).SortByDescending(x => x.CreatedAt), paginationParams.PageNumber, paginationParams.PageSize);
    }

    public Task<PagedList<T>> FilterByAsync(Expression<Func<T, bool>> filterExpression, PaginationParams paginationParams)
    {
        return Task.Run(() => PagedList<T>.ToPagedList(_collection.Find(filterExpression).SortByDescending(x => x.CreatedAt), paginationParams.PageNumber, paginationParams.PageSize));
    }

    public PagedList<TProjected> FilterBy<TProjected>(Expression<Func<T, bool>> filterExpression, PaginationParams paginationParams, Expression<Func<T, TProjected>> projectionExpression)
    {
        return PagedList<T>.ToPagedList(_collection.Find(filterExpression).Project(projectionExpression), paginationParams.PageNumber, paginationParams.PageSize);
    }

    public Task<PagedList<TProjected>> FilterByAsync<TProjected>(Expression<Func<T, bool>> filterExpression, PaginationParams paginationParams, Expression<Func<T, TProjected>> projectionExpression)
    {
        return Task.Run(() => PagedList<T>.ToPagedList(_collection.Find(filterExpression).Project(projectionExpression), paginationParams.PageNumber, paginationParams.PageSize));
    }

    public T FindOne(Expression<Func<T, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).FirstOrDefault();
    }

    public T FindById(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
        return _collection.Find(filter).SingleOrDefault();
    }

    public void InsertOne(T document)
    {
        _collection.InsertOne(document);
    }

    public void InsertMany(ICollection<T> documents)
    {
        _collection.InsertMany(documents);
    }

    public void ReplaceOne(T document)
    {
        var filter = Builders<T>.Filter.Eq(doc => doc.Id, document.Id);
        _collection.FindOneAndReplace(filter, document);
    }

    public void DeleteOne(Expression<Func<T, bool>> filterExpression)
    {
        _collection.FindOneAndDelete(filterExpression);
    }

    public Task DeleteOneAsync(Expression<Func<T, bool>> filterExpression)
    {
        return _collection.FindOneAndDeleteAsync(filterExpression);
    }

    public void DeleteById(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
        _collection.FindOneAndDelete(filter);
    }

    public void DeleteMany(Expression<Func<T, bool>> filterExpression)
    {
        _collection.DeleteMany(filterExpression);
    }

    public Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression)
    {
        return _collection.DeleteManyAsync(filterExpression);
    }

    public Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        return _collection.UpdateManyAsync(filter, update);
    }
}

