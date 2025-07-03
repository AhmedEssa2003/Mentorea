
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Mentorea.Services
{
    public class DistributedCacheService(IDistributedCache distributedCache) : IDistributedCacheService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken) where T : class
        {
            var jsonValue = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(jsonValue))
                return  null;
            return JsonSerializer.Deserialize<T>(jsonValue);
        }
        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
        {
            string jesonValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key,jesonValue,cancellationToken);
            
        }
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
    }
}
