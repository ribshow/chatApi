using chatApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace chatApi.Services
{
    public class ChatTechService
    {
        private readonly IMongoCollection<ChatTech> _chatCollection;

        private readonly IMongoDatabase _database;

        public ChatTechService(IOptions<ContextMongoDb> chatDatabaseSettings)
        {
            var mongoClient = new MongoClient(chatDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(chatDatabaseSettings.Value.DatabaseName);

            _database = mongoDatabase;

            _chatCollection = mongoDatabase.GetCollection<ChatTech>(chatDatabaseSettings.Value.ChatDSMCollection);
        }

        public IMongoCollection<ChatTech> ChatTech => _chatCollection; 

        public IMongoCollection<ChatTech> Chats => _database.GetCollection<ChatTech>("ChatTech");

        // retorna todas as mensagens do chat
        public async Task<List<ChatTech>> GetAsync() =>
            await _chatCollection.Find(chat => true).ToListAsync();

        // retorna uma mensagem específica
        public async Task<ChatTech?> GetAsync(string id) =>
            await _chatCollection.Find(chat => chat.Id == id).FirstOrDefaultAsync();

        // salva uma nova mensagem no banco de dados
        public async Task CreateAsync(ChatTech newChat) =>
            await _chatCollection.InsertOneAsync(newChat);

        // atualiza uma mensagem enviada
        public async Task UpdateAsync(string id, ChatTech updateChat) =>
            await _chatCollection.ReplaceOneAsync(chat => chat.Id == id, updateChat);

        // apaga uma mensagem enviada
        public async Task RemoveAsync(string id) =>
            await _chatCollection.DeleteOneAsync(chat => chat.Id == id);
    }
}
