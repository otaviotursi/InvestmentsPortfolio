using Users.Command;
using Users.Repository.Interface;
using Infrastructure.Repository.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Users.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserDomain> _eventCollection;
        public UserRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<UserDomain>(collectionName);
        }

        public async Task DeleteAsync(ulong id, CancellationToken cancellationToken)
        {
            var filter = Builders<UserDomain>.Filter.
                                     Where(x => x.Id.Equals(id));

            await _eventCollection.DeleteOneAsync(filter, null, cancellationToken);
        }

        public Task<List<UserDomain>> GetAll(CancellationToken cancellationToken)
        {
            return _eventCollection.Find(Builders<UserDomain>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public async Task<UserDomain> GetBy(string? user, string? fullName, ulong? id, CancellationToken cancellationToken)
        {
            var filter = Builders<UserDomain>.Filter.Empty;
            if (user != null) { 
                filter = Builders<UserDomain>.Filter.Eq(x => x.User, user);
            }
            else if (fullName != null)
            {
                filter = Builders<UserDomain>.Filter.Eq(x => x.FullName, fullName);
            }
            else if (id != null)
            {
                filter = Builders<UserDomain>.Filter.Eq(x => x.Id, id);
            }

            var result = await _eventCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }



        public async Task InsertAsync(UserDomain User, CancellationToken cancellationToken)
        {
            ulong id = GetDocumentWithMaxId().Result + 1 ;
            User.Id = id;
            await _eventCollection.InsertOneAsync(User);
        }

        public async Task UpdateAsync(UserDomain User, CancellationToken cancellationToken)
        {
            var filter = Builders<UserDomain>.Filter.
                                     Where(x => x.Id.Equals(User.Id));
            var update = Builders<UserDomain>.Update
                .Set(x => x.FullName, User.FullName)
                .Set(x => x.User, User.User);

            await _eventCollection.UpdateOneAsync(filter, update, null, cancellationToken);

        }

        public async Task<ulong> GetDocumentWithMaxId()
        {

            var documentWithMaxId = await _eventCollection
                .Find(new BsonDocument())
                .Sort(Builders<UserDomain>.Sort.Descending("id"))
                .ToListAsync();

            var lastId = documentWithMaxId.LastOrDefault()?.Id ?? 0;

            return lastId;
        }

    }
}
