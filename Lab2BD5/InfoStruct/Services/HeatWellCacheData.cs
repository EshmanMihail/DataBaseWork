using Lab2BD5;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoStruct.Services
{
    public class HeatWellCacheData : CacheService<HeatWell>
    {
        public HeatWellCacheData(HeatSchemeStorageContext context, IMemoryCache cache, IServiceProvider serviceProvider)
            : base(context, cache, serviceProvider)
        {
        }
    }
}
