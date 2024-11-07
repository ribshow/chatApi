using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using chatApi.Models;

namespace chatApi.Models
{
    public class ChatGeek
    {
        // torna id a chave primária do documento
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string? Fullname { get; set; }

        [BsonElement("Nickname")]
        public string? Nickname { get; set; }

        [BsonElement("Message")]
        public string? Message { get; set; }

        [BsonElement("Date_time")]
        public DateTime date { get; set; } = DateTime.UtcNow;

        public ChatGeek(string fullName, string nickname, string message)
        {
            Fullname = fullName;
            Nickname = nickname;
            Message = message;

            DateTime date = DateTime.UtcNow;
            DateTime saoPauloTime = TimeZoneConfig.ConvertToSaoPauloTime(date);
            this.date = saoPauloTime;
        }
    }
}
