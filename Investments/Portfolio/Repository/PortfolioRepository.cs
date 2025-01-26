using Infrastructure.Repository.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Portfolio.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly IMongoCollection<PortfolioDomain> _eventCollection;
        public PortfolioRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<PortfolioDomain>(collectionName);
        }

        public async Task<List<PortfolioDomain>> GetAll(CancellationToken cancellationToken)
        {
            return await _eventCollection.Find(Builders<PortfolioDomain>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public async Task<PortfolioDomain> GetById(ulong customerId, CancellationToken cancellationToken)
        {
            var filter = Builders<PortfolioDomain>.Filter.Eq(x => x.CustomerId, customerId);

            var result = await _eventCollection.FindAsync(filter);

            return result.FirstOrDefault();
        }
        public async Task InsertAsync(PortfolioRequest product, CancellationToken cancellationToken)
        {
            var filter = Builders<PortfolioDomain>.Filter.And(
                Builders<PortfolioDomain>.Filter.Eq(x => x.CustomerId, product.CustomerId),
                Builders<PortfolioDomain>.Filter.ElemMatch(x => x.ItensPortfolio, item => item.ProductId == product.ProductId)
            );
            var result = await _eventCollection.FindAsync(filter);

            var hasItem = result.FirstOrDefault();

            // Se o item não foi encontrado, adiciona um novo item na lista
            if (hasItem == null)
            {
                var addNewItem = Builders<PortfolioDomain>.Update.Push(x => x.ItensPortfolio, new ItemPortfolio
                {
                    Id = ObjectId.GenerateNewId(),
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    AmountNegotiated = product.AmountNegotiated,
                    ValueNegotiated = product.ValueNegotiated ?? 0, // Definindo como 0 se for nulo
                });

                var filterCustomerOnly = Builders<PortfolioDomain>.Filter.Eq(x => x.CustomerId, product.CustomerId);
                await _eventCollection.UpdateOneAsync(filterCustomerOnly, addNewItem, new UpdateOptions { IsUpsert = true }, cancellationToken);
            }
            else
            {
                await UpdateAsync(product);
            }
        }


        private async Task UpdateAsync(PortfolioRequest product)
        {
            var filterBase = Builders<PortfolioDomain>.Filter.Eq(x => x.CustomerId, product.CustomerId);

            // Busca o documento
            var portfolio = await _eventCollection.Find(filterBase).FirstOrDefaultAsync();

            if (portfolio != null)
            {
                // Procura o item na lista
                var item = portfolio.ItensPortfolio.FirstOrDefault(i => i.ProductId == product.ProductId);

                if (item != null)
                {
                    // Atualiza o item existente
                    item.AmountNegotiated += product.AmountNegotiated;
                    item.ProductName = product.ProductName;
                    item.ValueNegotiated = product.ValueNegotiated ?? item.ValueNegotiated;
                }
                else
                {
                    // Adiciona um novo item
                    portfolio.ItensPortfolio.Add(new ItemPortfolio
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        AmountNegotiated = product.AmountNegotiated,
                        ValueNegotiated = product.ValueNegotiated ?? 0
                    });
                }

                // Atualiza o documento no MongoDB
                var updateResult = await _eventCollection.ReplaceOneAsync(
                    filterBase,
                    portfolio
                );
            }
        }

        public async Task RemoveAsync(PortfolioRequest product, CancellationToken cancellationToken)
        {
            // Filtrar o documento base pelo CustomerId
            var filterBase = Builders<PortfolioDomain>.Filter.Eq(x => x.CustomerId, product.CustomerId);

            // Buscar o documento inteiro
            var portfolio = await _eventCollection.Find(filterBase).FirstOrDefaultAsync(cancellationToken);

            if (portfolio != null)
            {
                // Localizar o item na lista
                var item = portfolio.ItensPortfolio.FirstOrDefault(i => i.ProductId == product.ProductId);

                if (item != null)
                {
                    // Atualizar o AmountNegotiated decrementando o valor
                    item.AmountNegotiated -= product.AmountNegotiated;

                    // Se AmountNegotiated for menor ou igual a zero, remova o item da lista
                    if (item.AmountNegotiated <= 0)
                    {
                        portfolio.ItensPortfolio.Remove(item);
                    }
                }

                // Se a lista de ItensPortfolio estiver vazia após a remoção, exclua o documento inteiro
                if (portfolio.ItensPortfolio.Count == 0)
                {
                    await _eventCollection.DeleteOneAsync(filterBase, cancellationToken);
                }
                else
                {
                    // Caso contrário, atualize o documento completo no banco
                    await _eventCollection.ReplaceOneAsync(filterBase, portfolio, cancellationToken: cancellationToken);
                }
            }

        }

    }
}
