using MessageService.Dto;
using MessageService.Model;

namespace MessageService.Profiles;

public class ConversationProfile : AutoMapper.Profile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationResponse>();
    }
}