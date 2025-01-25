using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Cache
{
    internal sealed class CacheHelper(IDistributedCache _distributedCache) : ICacheHelper
    {
        public async Task SetDataAsync(string key, int seconds,string data)
        {
            var options = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(seconds) };

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(data), options);
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var cachedData = await _distributedCache.GetStringAsync(key);

            if (!string.IsNullOrWhiteSpace(cachedData))
            {
                return JsonConvert.DeserializeObject<T>(cachedData);
            }

            return default;
        }

        public async Task RemoveDataAsync(string key) => await _distributedCache.RemoveAsync(key);
    }
}
