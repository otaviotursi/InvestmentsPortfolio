using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Cache
{
    internal sealed class CacheHelper(IDistributedCache _distributedCache) : ICacheHelper
    {
        public async Task SetDataAsync<T>(string key, int seconds, T data)
        {
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(seconds)
            };

            var serializedData = JsonConvert.SerializeObject(data);

            await _distributedCache.SetStringAsync(key, serializedData, options);
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {

            var cachedData = await _distributedCache.GetStringAsync(key);

            try
            {


                if (string.IsNullOrWhiteSpace(cachedData) || cachedData == "[]")
                {
                    return default; // Retorna null se o JSON for vazio ou não existir
                }

                return JsonConvert.DeserializeObject<T>(cachedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao buscar dados no cache", cachedData);
                throw;
            }

        }

        public async Task RemoveDataAsync(string key) => await _distributedCache.RemoveAsync(key);
    }
}
