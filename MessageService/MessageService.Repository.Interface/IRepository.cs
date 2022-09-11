using System.Linq.Expressions;

using MongoDB.Driver;

using MessageService.Model.MongoDB;
using MessageService.Repository.Interface.Pagination;

namespace MessageService.Repository.Interface;

public interface IRepository<T> where T : IDocument
{
    IQueryable<T> AsQueryable();

    PagedList<T> FilterBy(
        Expression<Func<T, bool>> filterExpression,
        PaginationParams paginationParams);

    Task<PagedList<T>> FilterByAsync(
        Expression<Func<T, bool>> filterExpression,
        PaginationParams paginationParams);

    PagedList<TProjected> FilterBy<TProjected>(
        Expression<Func<T, bool>> filterExpression,
        PaginationParams paginationParams,
        Expression<Func<T, TProjected>> projectionExpression);

    Task<PagedList<TProjected>> FilterByAsync<TProjected>(
        Expression<Func<T, bool>> filterExpression,
        PaginationParams paginationParams,
        Expression<Func<T, TProjected>> projectionExpression);

    T FindOne(Expression<Func<T, bool>> filterExpression);

    Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression);

    T FindById(string id);

    Task<T> FindByIdAsync(string id);

    void InsertOne(T document);

    Task InsertOneAsync(T document);

    void InsertMany(ICollection<T> documents);

    Task InsertManyAsync(ICollection<T> documents);

    void ReplaceOne(T document);

    Task ReplaceOneAsync(T document);

    Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update);

    void DeleteOne(Expression<Func<T, bool>> filterExpression);

    Task DeleteOneAsync(Expression<Func<T, bool>> filterExpression);

    void DeleteById(string id);

    Task DeleteByIdAsync(string id);

    void DeleteMany(Expression<Func<T, bool>> filterExpression);

    Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression);
}

