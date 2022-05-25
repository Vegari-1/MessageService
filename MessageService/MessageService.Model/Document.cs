using MongoDB.Bson;

namespace MessageService.Model
{
	public class Document : IDocument
	{
		public ObjectId Id { get; set; }

		public DateTime CreatedAt => Id.CreationTime;
    }
}

