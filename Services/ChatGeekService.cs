using chatApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace chatApi.Services
{
    public class ChatGeekService
    {
        private readonly IMongoCollection<ChatGeek> _chatCollection;

        private readonly IMongoDatabase _database;

        public ChatGeekService(IOptions<ContextMongoDb> chatDatabaseSettings)
        {
            var mongoClient = new MongoClient(chatDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(chatDatabaseSettings.Value.DatabaseName);

            _database = mongoDatabase;

            _chatCollection = mongoDatabase.GetCollection<ChatGeek>(chatDatabaseSettings.Value.ChatGeekCollection);
        }

        public IMongoCollection<ChatGeek> Chats => _chatCollection;

        public IMongoCollection<ChatGeek> ChatGeek => _database.GetCollection<ChatGeek>("ChatGeek");

        // retorna todas as mensagens do chat
        public async Task<List<ChatGeek>> GetAsync() =>
            await _chatCollection.Find(chat => true).ToListAsync();

        // retorna uma mensagem específica
        public async Task<ChatGeek?> GetAsync(string id) =>
            await _chatCollection.Find(chat => chat.Id == id).FirstOrDefaultAsync();

        // salva uma nova mensagem no banco de dados
        public async Task CreateAsync(ChatGeek newChat) =>
            await _chatCollection.InsertOneAsync(newChat);

        // atualiza uma mensagem enviada
        public async Task UpdateAsync(string id, ChatGeek updateChat) =>
            await _chatCollection.ReplaceOneAsync(chat => chat.Id == id, updateChat);

        // apaga uma mensagem enviada
        public async Task RemoveAsync(string id) =>
            await _chatCollection.DeleteOneAsync(chat => chat.Id == id);
    }
}
