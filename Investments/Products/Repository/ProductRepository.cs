using Infrastructure.Repository.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<ProductDomain> _eventCollection;
        public ProductRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<ProductDomain>(collectionName);
        }

        public Task<List<ProductDomain>> GetAll(CancellationToken cancellationToken)
        {
            return _eventCollection.Find(Builders<ProductDomain>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<ProductDomain>.Filter.
                                     Where(x => x.Id.Equals(id));

            await _eventCollection.DeleteOneAsync(filter, null, cancellationToken);
        }


        public async Task InsertAsync(ProductDomain ProductDomain, CancellationToken cancellationToken)
        {
            var productById = await GetById(ProductDomain.Id, cancellationToken);
            if (productById == null) { 
                await _eventCollection.InsertOneAsync(ProductDomain);
            } else
            {
                await UpdateAsync(ProductDomain, cancellationToken);
            }

            
        }

        public async Task UpdateAsync(ProductDomain ProductDomain, CancellationToken cancellationToken)
        {
            var filter = Builders<ProductDomain>.Filter.Eq(x => x.Id, ProductDomain.Id);
            var updateDefinitionBuilder = Builders<ProductDomain>.Update;
            var updateDefinitions = new List<UpdateDefinition<ProductDomain>>();

            // Verifique cada campo e adicione ao UpdateDefinition se não for null
            if (ProductDomain?.Name != null)
                updateDefinitions.Add(updateDefinitionBuilder.Set(x => x.Name, ProductDomain.Name));

            if (ProductDomain?.UnitPrice != null)
                updateDefinitions.Add(updateDefinitionBuilder.Set(x => x.UnitPrice, ProductDomain.UnitPrice));

            if (ProductDomain?.ProductType != null)
                updateDefinitions.Add(updateDefinitionBuilder.Set(x => x.ProductType, ProductDomain.ProductType));

            if (ProductDomain?.AvailableQuantity != null)
                updateDefinitions.Add(updateDefinitionBuilder.Set(x => x.AvailableQuantity, ProductDomain.AvailableQuantity));

            // Combine todas as definições de atualização em um único UpdateDefinition
            var update = updateDefinitionBuilder.Combine(updateDefinitions);

            // Execute a atualização somente se houver algo a atualizar
            if (updateDefinitions.Any())
            {
                await _eventCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            }

        }

        public async Task<ProductDomain> GetByName(string name, CancellationToken cancellationToken)
        {
            var filter = Builders<ProductDomain>.Filter.Eq(x => x.Name, name);

            var result = await _eventCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            return result;
        }
        public Task<ProductDomain?> GetById(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<ProductDomain>.Filter.Eq(x => x.Id, id);

            var result = _eventCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            return result;
        }

        public Task<List<ProductDomain>> GetExpiritionByDateAll(int expirationDay, CancellationToken cancellationToken)
        {
            DateTime dataAtual = DateTime.Now;
            DateTime dataLimite = dataAtual.AddDays(expirationDay);

            var filter = Builders<ProductDomain>.Filter.Gte(x => x.ExpirationDate, dataAtual) &
                         Builders<ProductDomain>.Filter.Lte(x => x.ExpirationDate, dataLimite);

            var result = _eventCollection.Find(filter).ToListAsync(cancellationToken);

            return result;
        }
    }
}
