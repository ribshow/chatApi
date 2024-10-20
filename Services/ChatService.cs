using chatApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace chatApi.Services
{
    public class ChatService
    {
        private readonly IMongoCollection<Chat> _chatCollection;

        public ChatService(IOptions<ChatDatabaseSettings> chatDatabaseSettings)
        {
            var mongoClient = new MongoClient(chatDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(chatDatabaseSettings.Value.DatabaseName);

            _chatCollection = mongoDatabase.GetCollection<Chat>(chatDatabaseSettings.Value.ChatCollectionName);
        }

        // retorna todas as mensagens do chat
        public async Task<List<Chat>> GetAsync() =>
            await _chatCollection.Find(chat => true).ToListAsync();

        // retorna uma mensagem específica
        public async Task<Chat?> GetAsync(string id) =>
            await _chatCollection.Find(chat => chat.Id == id).FirstOrDefaultAsync();

        // salva uma nova mensagem no banco de dados
        public async Task CreateAsync(Chat newChat) =>
            await _chatCollection.InsertOneAsync(newChat);

        // atualiza uma mensagem enviada
        public async Task UpdateAsync(string id, Chat updateChat) =>
            await _chatCollection.ReplaceOneAsync(chat => chat.Id == id, updateChat);

        // apaga uma mensagem enviada
        public async Task RemoveAsync(string id) =>
            await _chatCollection.DeleteOneAsync(chat => chat.Id == id);
    }
}
