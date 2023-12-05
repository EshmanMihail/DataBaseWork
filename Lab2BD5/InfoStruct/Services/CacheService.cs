using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab2BD5;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace InfoStruct.Services
{
    public abstract class CacheService<T> where T : class, ITable
    {
        protected const int rowsNumber = 20;
        protected HeatSchemeStorageContext _heatSchemeStorageContext;
        protected IMemoryCache _cache;
        protected readonly IServiceProvider _serviceProvider;

        public CacheService(HeatSchemeStorageContext context, IMemoryCache cache, IServiceProvider serviceProvider)
        {
            _heatSchemeStorageContext = context;
            _cache = cache;
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<T>? Get(string cacheKey)
        {
            if (_cache.TryGetValue(cacheKey, out IEnumerable<T> result))
            {

                Console.WriteLine("Взято из кеша");
                return result;
            }

            result = _heatSchemeStorageContext
                .Set<T>()
                .AsEnumerable()
                .OrderBy(x => x.ID)
                .Take(rowsNumber)
                .ToList();

            if (result is not null)
            {
                _cache.Set(cacheKey, result, _serviceProvider.GetService<MemoryCacheEntryOptions>());
            }
            Console.WriteLine("Взято из бд");
            return result;
        }
    }
}
