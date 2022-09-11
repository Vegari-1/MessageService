using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace MessageService.Model.MongoDB;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    [JsonConverter(typeof(ObjectIdConverter))]
    ObjectId Id { get; set; }

    DateTime CreatedAt { get; }
}


