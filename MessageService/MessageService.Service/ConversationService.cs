using MessageService.Model;
using MessageService.Repository.Interface;
using MessageService.Repository.Interface.Pagination;
using MessageService.Service.Interface;
using MessageService.Service.Interface.Exceptions;
using MongoDB.Bson;

namespace MessageService.Service;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;

    public ConversationService(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public Task<PagedList<Conversation>> Filter(Guid id, PaginationParams paginationParams, string? query)
    {
        return _conversationRepository.FilterOrderedAsync(id, paginationParams, query);
    }

    public Task<Conversation> Get(string id)
    {
        return _conversationRepository.FindByIdAsync(id);
    }

    public Task Delete(string id)
    {
        return _conversationRepository.DeleteByIdAsync(id);
    }

    public async Task<Message> SendMessage(Conversation conversation, string senderId, string content)
    {
        var message = new Message()
        {
            Id = ObjectId.GenerateNewId(),
            Content = content,
            Sender = Guid.Parse(senderId)
        };

        if ((await _conversationRepository.AddMessage(conversation, message)).IsAcknowledged)
            return message;

        throw new InvalidOperationException();
    }

    public async Task<Conversation> StartConversation(Profile sender, Profile receiver)
    {
        var checkResult = await _conversationRepository.FindOneAsync(c =>
            c.Participants.Any(x => x.GlobalId == sender.GlobalId) &&
            c.Participants.Any(x => x.GlobalId == receiver.GlobalId));

        if (checkResult != null)
            throw new EntityAlreadyExistsException(typeof(Conversation));

        var conversation = new Conversation()
        {
            Participants = new List<Profile>() { sender, receiver }
        };

        await _conversationRepository.InsertOneAsync(conversation);
        return conversation;
    }
}

