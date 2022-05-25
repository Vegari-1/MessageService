using MessageService.Model;
using MessageService.Repository.Interface;

namespace MessageService.Repository;
public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(IMongoDbSettings settings) : base(settings)
    {
    }
}

