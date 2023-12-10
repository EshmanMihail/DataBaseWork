using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using ModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace InfoStruct.Service
{
    public class HeatWellCacheData : CacheService<HeatWell>
    {
        public HeatWellCacheData(HeatSchemeStorageContext context, IMemoryCache cache, IServiceProvider serviceProvider)
            : base(context, cache, serviceProvider)
        {
        }
    }
}

