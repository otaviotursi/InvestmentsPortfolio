using Infrastructure.Enum;
using Infrastructure.Repository.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Repository
{
    public class ProductStatementRepository
    : IProductStatementRepository
    {

        private readonly IMongoCollection<BsonDocument> _eventCollection;

        public ProductStatementRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<BsonDocument>(collectionName);

        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            ProductDomain ProductDomain = new(id);
            ProductDomain.Type = "Delete";
            var eventDocument = new BsonDocument
            {
                {"AggregateId", Guid.NewGuid().ToString()},
                {"Data", ProductDomain.ToBsonDocument()},
                {"TypeOperation", TypeOperationEnum.Delete.Name},
                {"TimeStamp", DateTime.UtcNow},
            };

            await _eventCollection.InsertOneAsync(eventDocument);
        }

        public async Task<List<ProductDomain>> GetStatementBy(string? name, ulong? userId, DateTime? expirationDate,Guid? productId, CancellationToken cancellationToken)
        {
            var filters = new List<FilterDefinition<BsonDocument>>();
            
            if (!string.IsNullOrEmpty(productId.ToString()))
            {
                filters.Add(Builders<BsonDocument>.Filter.Eq("Data.Id", name));
            }

            // Verifica se 'name' não é null e adiciona o filtro correspondente
            if (!string.IsNullOrEmpty(name))
            {
                filters.Add(Builders<BsonDocument>.Filter.Eq("Data.Name", name));
            }

            // Verifica se 'user' não é null e adiciona o filtro correspondente
            if (userId.HasValue)
            {
                filters.Add(Builders<BsonDocument>.Filter.Eq("Data.UserId", userId)); // Supondo que o campo seja 'Data.User'
            }

            // Verifica se 'expirationDate' não é null e adiciona o filtro correspondente
            if (expirationDate != null)
            {
                filters.Add(Builders<BsonDocument>.Filter.Gte("Data.ExpirationDate", expirationDate?.AddDays(-1)));
                filters.Add(Builders<BsonDocument>.Filter.Lte("Data.ExpirationDate", expirationDate?.AddDays(1)));
            }

            // Combina os filtros usando o operador AND se houver mais de um
            var filter = filters.Count > 0 ? Builders<BsonDocument>.Filter.And(filters) : FilterDefinition<BsonDocument>.Empty;

            var documents = _eventCollection.Find(filter).ToList();
            // Convertendo os resultados para ProductDomain
            List<ProductDomain> products = new List<ProductDomain>();
            foreach (var document in documents)
            {
                var product = new ProductDomain
                {
                    Id = Guid.Parse(document["Data"]["_id"].AsGuid.ToString()),
                    Name = document.Contains("Data") &&
                          document["Data"].IsBsonDocument &&
                          document["Data"].AsBsonDocument.Contains("Name") &&
                          !document["Data"]["Name"].IsBsonNull
                            ? document["Data"]["Name"].AsString
                            : string.Empty,
                    UserId = Convert.ToUInt64(document["Data"]["UserId"]),
                    UnitPrice = Convert.ToDecimal(document["Data"]["UnitPrice"].AsString),
                    ExpirationDate = document["Data"]["ExpirationDate"].ToUniversalTime(),
                    ProductType = document.Contains("Data") &&
                          document["Data"].IsBsonDocument &&
                          document["Data"].AsBsonDocument.Contains("ProductType") &&
                          !document["Data"]["ProductType"].IsBsonNull
                            ? document["Data"]["ProductType"].AsString
                            : string.Empty,
                    Type = document.Contains("Data") &&
                          document["Data"].IsBsonDocument &&
                          document["Data"].AsBsonDocument.Contains("Type") &&
                          !document["Data"]["Type"].IsBsonNull
                            ? document["Data"]["Type"].AsString
                            : string.Empty,
                    AvailableQuantity = Convert.ToInt32(document["Data"]["AvailableQuantity"].AsString)
                };

                products.Add(product);
            }

            return products;
        }

        public async Task InsertAsync(ProductDomain ProductDomain, CancellationToken cancellationToken)
        {
            var eventDocument = new BsonDocument
            {
                {"AggregateId", Guid.NewGuid().ToString()},
                {"Data", ProductDomain.ToBsonDocument()},
                {"TypeOperation", TypeOperationEnum.Insert.Name},
                {"TimeStamp", DateTime.UtcNow},
            };

            await _eventCollection.InsertOneAsync(eventDocument);
        }

        public async Task UpdateAsync(ProductDomain ProductDomain, CancellationToken cancellationToken)
        {
            var eventDocument = new BsonDocument
            {
                {"AggregateId", Guid.NewGuid().ToString()},
                {"Data", ProductDomain.ToBsonDocument()},
                {"TypeOperation", TypeOperationEnum.Update.Name},
                {"TimeStamp", DateTime.UtcNow},
            };

            await _eventCollection.InsertOneAsync(eventDocument);
        }
    }
}
