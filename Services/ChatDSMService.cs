using chatApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace chatApi.Services
{
    public class ChatDSMService
    {
        private readonly IMongoCollection<ChatDSM> _chatCollection;

        private readonly IMongoDatabase _database;

        public ChatDSMService(IOptions<ContextMongoDb> chatDatabaseSettings)
        {
            var mongoClient = new MongoClient(chatDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(chatDatabaseSettings.Value.DatabaseName);

            _database = mongoDatabase;

            _chatCollection = mongoDatabase.GetCollection<ChatDSM>(chatDatabaseSettings.Value.ChatDSMCollection);
        }

        public IMongoCollection<ChatDSM> ChatDSM => _chatCollection; 

        public IMongoCollection<ChatDSM> Chats => _database.GetCollection<ChatDSM>("ChatDSM");

        // retorna todas as mensagens do chat
        public async Task<List<ChatDSM>> GetAsync() =>
            await _chatCollection.Find(chat => true).ToListAsync();

        // retorna uma mensagem específica
        public async Task<ChatDSM?> GetAsync(string id) =>
            await _chatCollection.Find(chat => chat.Id == id).FirstOrDefaultAsync();

        // salva uma nova mensagem no banco de dados
        public async Task CreateAsync(ChatDSM newChat) =>
            await _chatCollection.InsertOneAsync(newChat);

        // atualiza uma mensagem enviada
        public async Task UpdateAsync(string id, ChatDSM updateChat) =>
            await _chatCollection.ReplaceOneAsync(chat => chat.Id == id, updateChat);

        // apaga uma mensagem enviada
        public async Task RemoveAsync(string id) =>
            await _chatCollection.DeleteOneAsync(chat => chat.Id == id);
    }
}
