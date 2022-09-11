namespace MessageService.Dto
{
    public class MessageResponse
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid Sender { get; set; }
        public string Content { get; set; }
    }
}

