using MessageService.Repository.Interface;
using MessageService.Service.Interface;

namespace MessageService.Service;
public class MessageService : IMessageService
{
    private readonly IMessageRepository _repository;

    public MessageService(IMessageRepository repository)
    {
        _repository = repository;
    }
}

