using MessageService.Model.MongoDB;

namespace MessageService.Model;

[BsonCollection("conversation")]
public class Conversation : Document
{
    public ICollection<Profile> Participants { get; set; } = new List<Profile>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}