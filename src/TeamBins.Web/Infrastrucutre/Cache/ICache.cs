using System;

namespace TeamBins.Infrastrucutre.Cache
{
    public interface ICache
    {
        /// <summary>
        /// Gets data from the cache if exists, else executes the code passed to get data and set to cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">cache key</param>
        /// <param name="updateExpression">Optional lamda expression to exececute to load data to cache</param>
        /// <param name="cacheDuration">Cache duration in seconds. If zero, I</param>
        /// <returns>T data</returns>
        T Get<T>(string key, Func<T> updateExpression = null, int cacheDuration=0);

        /// <summary>
        /// Remove the cache entry for the key passed
        /// </summary>
        /// <param name="key"></param>
        void Clear(string key);
    }
}