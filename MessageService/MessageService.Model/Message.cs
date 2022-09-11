using MessageService.Model.MongoDB;

namespace MessageService.Model;

public class Message : Document
{
    public Guid Sender { get; set; }
    public string Content { get; set; }
}