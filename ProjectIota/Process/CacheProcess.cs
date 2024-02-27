using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json;

namespace ProjectIota.Process
{
	public static class CacheProcess
	{
		public static async Task SetAsync<T>(this IDistributedCache cache, string recordId, T data, int expireSeconds, int slidingSeconds)
		{
			var options = new DistributedCacheEntryOptions();
			options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds);
			options.SlidingExpiration = TimeSpan.FromSeconds(slidingSeconds); //If in the given time data will not be accessed cache will be expire.

			var jsonData = JsonConvert.SerializeObject(data); //Use NewtonsoftJson instead of JsonSerializer
			await cache.SetStringAsync(recordId, jsonData, options);
		}

		public static async Task<T> GetAsync<T>(this IDistributedCache cache, string recordId)
		{
			var jsonData = await cache.GetStringAsync(recordId);

			if (jsonData is null)
			{
				return default(T);
			}

			return JsonConvert.DeserializeObject<T>(jsonData); //Use NewtonsoftJson instead of JsonSerializer
		}
	}
}
