using MongoDB.Driver;

using MessageService.Model;
using MessageService.Repository.Interface.Pagination;

namespace MessageService.Repository.Interface;

public interface IConversationRepository : IRepository<Conversation>
{
    public Task<PagedList<Conversation>> FilterOrderedAsync(Guid ownerId, PaginationParams paginationParams, string? query);
    public Task<UpdateResult> AddMessage(Conversation conversation, Message message);
}

