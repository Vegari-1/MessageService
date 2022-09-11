using MongoDB.Bson;

namespace MessageService.Model.MongoDB;

public class Document : IDocument
{
	public ObjectId Id { get; set; }

	public DateTime CreatedAt => Id.CreationTime;
	}

