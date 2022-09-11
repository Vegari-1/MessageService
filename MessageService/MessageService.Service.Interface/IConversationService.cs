using MessageService.Model;
using MessageService.Repository.Interface.Pagination;

namespace MessageService.Service.Interface;

public interface IConversationService
{
    Task<PagedList<Conversation>> Filter(Guid id, PaginationParams paginationParams, string? query);
    Task<Conversation> Get(string id);
    Task Delete(string id);
    Task<Conversation> StartConversation(Profile sender, Profile receiver);
    Task<Message> SendMessage(Conversation conversation, string senderId, string content);
}

