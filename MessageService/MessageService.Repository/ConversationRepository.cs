using MongoDB.Driver;

using MessageService.Model;
using MessageService.Repository.Interface;
using MessageService.Repository.Interface.Pagination;

namespace MessageService.Repository;

public class ConversationRepository : Repository<Conversation>, IConversationRepository
{
    public ConversationRepository(IMongoDbSettings settings) : base(settings)
    {
    }

    public Task<PagedList<Conversation>> FilterOrderedAsync(Guid ownerId, PaginationParams paginationParams, string? query)
    {
        return Task.Run(() =>
        {
            var builder = Builders<Conversation>.Filter;
            var filter = builder.ElemMatch(c => c.Participants, p => p.GlobalId == ownerId);

            if (!string.IsNullOrEmpty(query))
            {
                var q = query.ToLower();
                filter &= builder.Or(
                    builder.ElemMatch(c => c.Participants, p => p.Name.ToLower().Contains(q)),
                    builder.ElemMatch(c => c.Participants, p => p.Name.ToLower().Contains(q))
                );
            }

            var conversations = _collection.Find(filter).SortByDescending(c => c.Id);

            return PagedList<Conversation>.ToPagedList(conversations, paginationParams.PageNumber, paginationParams.PageSize);
        });
    }

    public async Task<UpdateResult> AddMessage(Conversation conversation, Message message)
    {
        var filter = Builders<Conversation>.Filter.Eq(x => x.Id, conversation.Id);
        var update = Builders<Conversation>.Update.Push(x => x.Messages, message);
        return await _collection.UpdateOneAsync(filter, update);
    }
}