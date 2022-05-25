using System.Linq.Expressions;
using MessageService.Model;

namespace MessageService.Repository.Interface
{
    public interface IRepository<T> where T : IDocument
    {
        Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression);
        Task<T> FindByIdAsync(string id);
        Task InsertOneAsync(T document);
        Task InsertManyAsync(ICollection<T> documents);
        Task ReplaceOneAsync(T document);
        Task DeleteByIdAsync(string id);
        IQueryable<T> AsQueryable();
    }
}

