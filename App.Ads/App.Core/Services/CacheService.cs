using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace App.Core.Services
{
    public interface ICacheService
    {
        void Clear(string cacheKey);
    }

    public class CacheService : ICacheService
    {
        void ICacheService.Clear(string cacheKey)
        {
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                HttpRuntime.Cache.Remove(cacheKey);
            }
        }

    }
}
