using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chatApi.Models
{
    public class ContextMongoDb
    {

        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ChatCollectionName { get; set; } = null!;
    }   
}
