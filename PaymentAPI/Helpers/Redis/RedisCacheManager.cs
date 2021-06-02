using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Helpers.Redis
{
    public class RedisCacheManager : IRedisCacheManager
    {
        private readonly ILogger<RedisCacheManager> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheManager(ILogger<RedisCacheManager> logger, ConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            //_database = redis.GetDatabase(2);
            _database = redis.GetDatabase(); //默认0号库
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }

        #region 异步

        #region String

        public async Task ClearAsync()
        {
            foreach (var endPoint in _redis.GetEndPoints())
            {
                var server = GetServer();
                foreach (var key in server.Keys())
                {
                    await _database.KeyDeleteAsync(key);
                }
            }
        }

        public async Task<bool> ExistAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<string> GetValueAsync(string key)
        {
            var result = await _database.StringGetAsync(key);
            return result;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return true;
            return await _database.KeyDeleteAsync(key);
        }

        public async Task SetAsync(string key, object value, TimeSpan? cacheTime = null)
        {
            if (value != null)
            {
                //序列化，将object值生成RedisValue
                await _database.StringSetAsync(key, SerializeHelper.Serialize(value), cacheTime);
            }
        }

        public async Task SetMultipleAsync(KeyValuePair<RedisKey, RedisValue>[] values)
        {
            if (values.Length > 0)
            {
                //序列化，将object值生成RedisValue
                await _database.StringSetAsync(values);
            }
        }

        public async Task<TEntity> GetEntityAsync<TEntity>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return SerializeHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default(TEntity);
            }
        }

        #endregion

        #endregion

    }
}
