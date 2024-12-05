using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace chatApi.Models
{
    public class Blacklist
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string? Fullname { get; set; }

        [BsonElement("Nickname")]
        public string? Nickname { get; set; }

        [BsonElement("Message")]
        public string? Message { get; set; }

        [BsonElement("Date")]
        public DateTime? Created { get; set; }

        [BsonElement("Origin")]
        public string? Origin { get; set; }

        public Blacklist(string? id, string? fullname, string? nickname, string? message, DateTime created, string? origin)
        {
            Id = id;
            Fullname = fullname;
            Nickname = nickname;
            Message = message;
            Created = created;
            Origin = origin;
        }
    }
}
