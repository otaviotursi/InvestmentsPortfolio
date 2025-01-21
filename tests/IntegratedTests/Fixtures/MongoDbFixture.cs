using Mongo2Go;
using MongoDB.Driver;

namespace IntegratedTests.Fixtures
{
    public class MongoDbFixture : IDisposable
    {
        private readonly MongoDbRunner _runner;
        public IMongoClient Client { get; }
        public string DatabaseName { get; }

        public MongoDbFixture()
        {
            _runner = MongoDbRunner.Start();
            Client = new MongoClient(_runner.ConnectionString);
            DatabaseName = "TestDb"; // Nome do banco de dados para testes
        }

        public void Dispose()
        {
            _runner.Dispose();
        }
    }
}
