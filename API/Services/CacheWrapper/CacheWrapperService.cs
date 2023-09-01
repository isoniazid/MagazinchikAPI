using MagazinchikAPI.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;

namespace MagazinchikAPI.Services.CacheWrapper
{
    public class CacheWrapperService : ICacheWrapperService
    {
        private readonly IDistributedCache _cache;
        public CacheWrapperService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<T> TryGetFromCacheAsync<T>(string recordKey)
        {

            var result = await _cache.GetRecordAsync<T>(recordKey);

            //NB need to add logger...
                if(result is not null)
                {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Loaded {recordKey} from cache");
                Console.ResetColor();
                }


            return result;
        }

        public async Task SaveToCacheAsync<T>(string recordKey, T value)
        {
            await _cache.SetRecordAsync(recordKey, value);
        }

        public async Task SaveToCacheAsync<T>(string recordKey, T value, TimeSpan? absoluteExpireTime = null,
         TimeSpan? unusedExpireTime = null)
        {
            await _cache.SetRecordAsync(recordKey, value, absoluteExpireTime, unusedExpireTime);
        }

        public string CreateRecordKey<T>(string prefix, T value, string timeFormat = "dd_MM_yyyy_hh")
        {
            return $"{prefix}__{DateTime.Now.ToString(timeFormat)}__{value}";
        }
    }
}