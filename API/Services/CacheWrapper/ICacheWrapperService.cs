namespace MagazinchikAPI.Services.CacheWrapper
{
    public interface ICacheWrapperService
    {
        public Task<T> TryGetFromCacheAsync<T>(string recordKey);


        public Task SaveToCacheAsync<T>(string recordKey, T value);


        public Task SaveToCacheAsync<T>(string recordKey, T value, TimeSpan?
         absoluteExpireTime = null, TimeSpan? unusedExpireTime = null);


        public string CreateRecordKey<T>(string prefix, T value, string timeFormat = "dd_MM_yyyy_hh");

    }
}