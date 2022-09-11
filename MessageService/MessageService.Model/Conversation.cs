using MessageService.Model.MongoDB;

namespace MessageService.Model;

[BsonCollection("conversation")]
public class Conversation : Document
{
    public List<Profile> Participants { get; set; } = new List<Profile>();
    public List<Message> Messages { get; set; } = new List<Message>();
}