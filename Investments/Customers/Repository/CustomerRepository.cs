using Customers.Command;
using Customers.Repository.Interface;
using Infrastructure.Repository.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Customers.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<CustomerDomain> _eventCollection;
        public CustomerRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<CustomerDomain>(collectionName);
        }

        public async Task DeleteAsync(ulong id, CancellationToken cancellationToken)
        {
            var filter = Builders<CustomerDomain>.Filter.
                                     Where(x => x.Id.Equals(id));

            await _eventCollection.DeleteOneAsync(filter, null, cancellationToken);
        }

        public Task<List<CustomerDomain>> GetAll(CancellationToken cancellationToken)
        {
            return _eventCollection.Find(Builders<CustomerDomain>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public async Task<CustomerDomain> GetBy(string? user, string? fullName, ulong? id, CancellationToken cancellationToken)
        {
            var filter = Builders<CustomerDomain>.Filter.Empty;
            if (user != null) { 
                filter = Builders<CustomerDomain>.Filter.Eq(x => x.User, user);
            }
            else if (fullName != null)
            {
                filter = Builders<CustomerDomain>.Filter.Eq(x => x.FullName, fullName);
            }
            else if (id != null)
            {
                filter = Builders<CustomerDomain>.Filter.Eq(x => x.Id, id);
            }

            var result = await _eventCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }



        public async Task InsertAsync(CustomerDomain customer, CancellationToken cancellationToken)
        {
            ulong id = GetDocumentWithMaxId().Result + 1 ;
            customer.Id = id;
            await _eventCollection.InsertOneAsync(customer);
        }

        public async Task UpdateAsync(CustomerDomain customer, CancellationToken cancellationToken)
        {
            var filter = Builders<CustomerDomain>.Filter.
                                     Where(x => x.Id.Equals(customer.Id));
            var update = Builders<CustomerDomain>.Update
                .Set(x => x.FullName, customer.FullName)
                .Set(x => x.User, customer.User);

            await _eventCollection.UpdateOneAsync(filter, update, null, cancellationToken);

        }

        public async Task<ulong> GetDocumentWithMaxId()
        {

            var documentWithMaxId = await _eventCollection
                .Find(new BsonDocument())
                .Sort(Builders<CustomerDomain>.Sort.Descending("id"))
                .ToListAsync();

            var lastId = documentWithMaxId.LastOrDefault()?.Id ?? 0;

            return lastId;
        }

    }
}
