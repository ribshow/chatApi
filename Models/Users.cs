using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace chatApi.Models
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Email")]
        [Required]
        [EmailAddress(ErrorMessage = "Digit a valid e-mail")]
        public string? Email { get; set; }

        [BsonElement("Password")]
        [Required(ErrorMessage = "Digit a valid password")]
        [StringLength(8, ErrorMessage = "Password must contain minimum 8 caracters")]
        public string? Password { get; set; }

        public Users(string? email, string? password)
        {
            Email = email;
            Password = password;
        }
    }
}
