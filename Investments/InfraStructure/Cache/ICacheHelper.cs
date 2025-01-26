using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Cache
{
    public interface ICacheHelper
    {
        Task SetDataAsync<T>(string key, int seconds, T data);


        Task<T?> GetDataAsync<T>(string key);
        

        Task RemoveDataAsync(string key);
    }
}
