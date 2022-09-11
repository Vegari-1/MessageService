namespace MessageService.Dto
{
    public class ConversationResponse
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<MessageResponse> messages { get; set; }
        public ICollection<ProfileResponse> participants { get; set; }
    }
}
