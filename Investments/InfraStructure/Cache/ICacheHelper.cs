using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Cache
{
    public interface ICacheHelper
    {
        Task SetDataAsync(string key, int seconds, string data);


        Task<T?> GetDataAsync<T>(string key);
        

        Task RemoveDataAsync(string key);
    }
}
