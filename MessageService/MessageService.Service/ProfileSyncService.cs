using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using BusService;
using BusService.Contracts;

using MessageService.Model;
using MessageService.Repository.Interface;
using MessageService.Service.Interface;

namespace MessageService.Service;
public class ProfileSyncService : ConsumerBase<Profile, ProfileContract>, IProfileSyncService
{
    private readonly IMessageBusService _messageBusService;
    private readonly IConversationRepository _conversationRepository;

    public ProfileSyncService(IMessageBusService messageBusService, IConversationRepository conversationRepository,
         ILogger<ProfileSyncService> logger) : base(logger)
    {
        _messageBusService = messageBusService;
        _conversationRepository = conversationRepository;
    }

    public override Task PublishAsync(Profile entity, string action)
    {
        // Implement when it is needed
        throw new NotImplementedException();
    }

    public override async Task SynchronizeAsync(ProfileContract entity, string action)
    {
        var filterMessages = Builders<Conversation>.Filter.ElemMatch(x => x.Participants, x => x.GlobalId == entity.Id);
        if (action == Events.Updated)
        {
            var updateJobOffer = Builders<Conversation>.Update
                .Set(x => x.Participants[-1].Name, entity.Name)
                .Set(x => x.Participants[-1].Surname, entity.Surname)
                .Set(x => x.Participants[-1].Avatar, entity.Avatar);
            await _conversationRepository.UpdateManyAsync(filterMessages, updateJobOffer);
        }
        else if (action == Events.Deleted)
        {
            var updateJobOffer = Builders<Conversation>.Update
                .Set(x => x.Participants[-1].Name, "Removed user")
                .Set(x => x.Participants[-1].Surname, "")
                .Set(x => x.Participants[-1].Avatar, "");
            await _conversationRepository.UpdateManyAsync(filterMessages, updateJobOffer);
        }
    }
}
