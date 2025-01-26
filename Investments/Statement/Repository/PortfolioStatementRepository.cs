using Infrastructure.Enum;
using Infrastructure.Repository.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Statement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statement.Repository
{
    public class PortfolioStatementRepository : IPortfolioStatementRepository
    {
        private readonly IMongoCollection<BsonDocument> _eventCollection;
        public PortfolioStatementRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<BsonDocument>(collectionName);
        }

        public async Task<List<PortfolioStatementDomain>> GetByCustomerId(ulong customerId, CancellationToken cancellationToken)
        {
            var filters = new List<FilterDefinition<BsonDocument>>();

            // Verifica se 'name' não é null e adiciona o filtro correspondente
            if (customerId == 0)
            {
                filters.Add(Builders<BsonDocument>.Filter.Eq("Data.CustomerId", customerId));
            }

            // Combina os filtros usando o operador AND se houver mais de um
            var filter = filters.Count > 0 ? Builders<BsonDocument>.Filter.And(filters) : FilterDefinition<BsonDocument>.Empty;

            var documents = _eventCollection.Find(filter).ToList();
            // Convertendo os resultados para ProductDomain
            List<PortfolioStatementDomain> portfolioList = new List<PortfolioStatementDomain>();
            foreach (var document in documents)
            {
                var portfolio = new PortfolioStatementDomain
                {
                    ProductId = document["Data"]["ProductId"].BsonType == MongoDB.Bson.BsonType.ObjectId
                    ? new Guid(document["Data"]["ProductId"].AsObjectId.ToByteArray())
                    : document["Data"]["ProductId"].AsGuid,
                    CustomerId = (ulong)document["Data"]["CustomerId"].AsInt64,
                    ProductName = document["Data"]["ProductName"].AsString,
                    AmountNegotiated = Convert.ToDecimal(document["Data"]["AmountNegotiated"].AsString),
                    OperationType = document["Data"]["OperationType"].AsString,
                    TransactionDate = document["Data"]["TransactionDate"].ToUniversalTime()
                };

                portfolioList.Add(portfolio);
            }

            return portfolioList;
        }

        public async Task InsertAsync(PortfolioStatementDomain? product, CancellationToken stoppingToken)
        {
            var eventDocument = new BsonDocument
            {
                {"AggregateId", Guid.NewGuid().ToString()},
                {"Data", product.ToBsonDocument()},
                {"TypeOperation", TypeOperationEnum.Insert.Name},
                {"TimeStamp", DateTime.UtcNow},
            };

            await _eventCollection.InsertOneAsync(eventDocument);
        }
    }
}
