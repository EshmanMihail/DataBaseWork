using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using ModelsLibrary.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoStruct.Service
{
    public class EnterpriceCacheData : CacheService<Enterprise>
    {
        public EnterpriceCacheData(HeatSchemeStorageContext context, IMemoryCache cache, IServiceProvider serviceProvider)
            : base(context, cache, serviceProvider)
        {
        }
    }
}
