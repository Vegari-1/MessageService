using MessageService.Dto;
using MessageService.Model;

namespace MessageService.Profiles
{
	public class MessageProfile : AutoMapper.Profile
	{
		public MessageProfile()
		{
			CreateMap<Message, MessageResponse>();
		}
	}
}

