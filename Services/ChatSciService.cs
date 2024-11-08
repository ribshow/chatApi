using chatApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace chatApi.Services
{
    public class ChatSciService
    {
        private readonly IMongoCollection<ChatSci> _chatCollection;

        private readonly IMongoDatabase _database;

        public ChatSciService(IOptions<ContextMongoDb> chatDatabaseSettings)
        {
            var mongoClient = new MongoClient(chatDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(chatDatabaseSettings.Value.DatabaseName);

            _database = mongoDatabase;

            _chatCollection = mongoDatabase.GetCollection<ChatSci>(chatDatabaseSettings.Value.ChatSciCollection);
        }

        public IMongoCollection<ChatSci> Chats => _chatCollection;

        public IMongoCollection<ChatSci> ChatSci => _database.GetCollection<ChatSci>("ChatSci");

        // retorna todas as mensagens do chat
        public async Task<List<ChatSci>> GetAsync() =>
            await _chatCollection.Find(chat => true).ToListAsync();

        // retorna uma mensagem específica
        public async Task<ChatSci?> GetAsync(string id) =>
            await _chatCollection.Find(chat => chat.Id == id).FirstOrDefaultAsync();

        // salva uma nova mensagem no banco de dados
        public async Task CreateAsync(ChatSci newChat) =>
            await _chatCollection.InsertOneAsync(newChat);

        // atualiza uma mensagem enviada
        public async Task UpdateAsync(string id, ChatSci updateChat) =>
            await _chatCollection.ReplaceOneAsync(chat => chat.Id == id, updateChat);

        // apaga uma mensagem enviada
        public async Task RemoveAsync(string id) =>
            await _chatCollection.DeleteOneAsync(chat => chat.Id == id);
    }
}
