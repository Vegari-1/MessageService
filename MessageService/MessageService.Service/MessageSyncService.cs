using System.Text;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using BusService;
using BusService.Contracts;
using BusService.Routing;

using MessageService.Model;
using MessageService.Repository.Interface;
using MessageService.Service.Interface;

namespace MessageService.Service;
public class MessageSyncService : ConsumerBase<MessageInfo, MessageContract>, IMessageSyncService
{
    private readonly IMessageBusService _messageBusService;

    public MessageSyncService(IMessageBusService messageBusService, IConversationRepository conversationRepository,
         ILogger<MessageSyncService> logger) : base(logger)
    {
        _messageBusService = messageBusService;
    }

    public override Task PublishAsync(MessageInfo entity, string action)
    {
        var contract = new MessageContract(entity.SenderId, entity.ReceiverId);

        var serialized = JsonConvert.SerializeObject(contract);
        var bData = Encoding.UTF8.GetBytes(serialized);
        _messageBusService.PublishEvent(SubjectBuilder.Build(Topics.Message, action), bData);
        return Task.CompletedTask;
    }

    public override Task SynchronizeAsync(MessageContract entity, string action)
    {
        // Implement when it is needed
        throw new NotImplementedException();
    }
}
