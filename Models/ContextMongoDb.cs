using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.ComponentModel;

namespace chatApi.Models
{
    public class ContextMongoDb
    {
        private readonly IMongoDatabase _database;

        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ChatCollectionName { get; set; } = null!;

        public string ChatTechCollection { get; set; } = null!;

        public string ChatGeekCollection { get; set; } = null!;

        public string ChatSciCollection { get; set; } = null!;

        public ContextMongoDb()
        {
            var stringConnection = "mongodb://localhost:27017";
            var database = "ChatFatec";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(stringConnection));

            var mongoClient = new MongoClient(settings);

            _database = mongoClient.GetDatabase(database);
        }

        public IMongoCollection<Users> Users => _database.GetCollection<Users>("Users");

        public IMongoCollection<Chat> Chat => _database.GetCollection<Chat>("Chat");

        public IMongoCollection<Blacklist> Blacklist => _database.GetCollection<Blacklist>("Blacklist");
    }   
}
