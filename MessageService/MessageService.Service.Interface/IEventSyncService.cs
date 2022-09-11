using BusService;
using BusService.Contracts;

namespace MessageService.Service.Interface
{
    public interface IEventSyncService : ISyncService<EventContract, EventContract>
    {
    }
}
