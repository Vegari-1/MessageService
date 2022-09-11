using MessageService.Model.MongoDB;

namespace MessageService.Model;

public class Profile
{
    public Guid GlobalId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Avatar { get; set; }
}
