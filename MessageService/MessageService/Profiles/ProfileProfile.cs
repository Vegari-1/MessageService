using MessageService.Dto;
using MessageService.Model;

namespace MessageService.Profiles;

public class ProfileProfile : AutoMapper.Profile
{
    public ProfileProfile()
    {
        // Source -> Target
        CreateMap<Profile, ProfileResponse>();
    }
}