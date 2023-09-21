using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WeatherApi.Settings;

namespace WeatherApi.Services
{
    public class MongoConnectionBuilder
    {
        private readonly IOptions<MongoConnectionSettings> _settings;
        public MongoConnectionBuilder(IOptions<MongoConnectionSettings> settings) 
        {
            _settings = settings;
        }

        public IMongoDatabase GetDatabase() 
        {
            var client = new MongoClient(_settings.Value.ConnectionString);
            return client.GetDatabase(_settings.Value.DatabaseName);
        }

        public IMongoDatabase GetDatabase(string database) 
        {
            var client = new MongoClient(_settings.Value.ConnectionString);
            return client.GetDatabase(database);
        }

        public IMongoDatabase GetDatabase(string connString, string database) 
        {
            var client = new MongoClient(connString);
            return client.GetDatabase(database);
        }
    }
}
