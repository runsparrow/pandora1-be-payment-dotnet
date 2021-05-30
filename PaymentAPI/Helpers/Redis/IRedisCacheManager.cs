using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Helpers.Redis
{
    public interface IRedisCacheManager
    {

        #region 异步

        #region String

        //获取 Reids 缓存值
        Task<string> GetValueAsync(string key);

        //获取值，并序列化
        Task<TEntity> GetEntityAsync<TEntity>(string key);

        //保存
        Task SetAsync(string key, object value, TimeSpan? cacheTime = null);
        //批量保存
        Task SetMultipleAsync(KeyValuePair<RedisKey, RedisValue>[] values);

        //判断是否存在
        Task<bool> ExistAsync(string key);

        //移除某一个缓存值
        Task<bool> DeleteAsync(string key);

        //全部清除
        Task ClearAsync();

        #endregion

        #endregion

    }
}