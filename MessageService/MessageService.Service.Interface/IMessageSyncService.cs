using BusService;
using BusService.Contracts;
using MessageService.Model;

namespace MessageService.Service.Interface;

public interface IMessageSyncService : ISyncService<MessageInfo, MessageContract>
{

}

