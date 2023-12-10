using Microsoft.Extensions.Caching.Memory;
using ModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InfoStruct.Service
{
    public class HeatConsumerCacheData : CacheService<HeatConsumer>
    {
        public HeatConsumerCacheData(HeatSchemeStorageContext context, IMemoryCache cache, IServiceProvider serviceProvider)
            : base(context, cache, serviceProvider)
        {
        }
    }
}

