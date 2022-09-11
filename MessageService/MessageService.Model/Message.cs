using MessageService.Model.MongoDB;

namespace MessageService.Model;

public class Message : Document
{
    public Profile Sender { get; set; }
    public string Content { get; set; }
    public bool Seen { get; set; }
}