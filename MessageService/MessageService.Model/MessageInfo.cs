namespace MessageService.Model;

public class MessageInfo
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }

    public MessageInfo(Guid senderId, Guid receiverId)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
    }
}
