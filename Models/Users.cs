using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace chatApi.Models
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Email")]
        public string? Email { get; set; }

        [BsonElement("Password")]
        public string? Password { get; set; }

        public Users(string? email, string? password)
        {
            Email = email;
            Password = password;
        }
    }
}
