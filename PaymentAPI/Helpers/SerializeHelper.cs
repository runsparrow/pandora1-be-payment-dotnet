using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Helpers
{
    public class SerializeHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);

            return Encoding.UTF8.GetBytes(jsonString);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEntity Deserialize<TEntity>(byte[] value)
        {
            if (value == null)
            {
                return default(TEntity);
            }
            var jsonString = Encoding.UTF8.GetString(value);
            return JsonConvert.DeserializeObject<TEntity>(jsonString);
        }

        public static List<TEntity> DeserializeList<TEntity>(List<byte[]> list)
        {
            List<TEntity> newList = new List<TEntity>();
            if (list == null)
            {
                return default(List<TEntity>);
            }

            foreach (var item in list)
            {
                var jsonString = Encoding.UTF8.GetString(item);
                newList.Add(JsonConvert.DeserializeObject<TEntity>(jsonString));
            }

            return newList;
        }

        /// <summary>
        ///  Serialize in Redis format
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HashEntry[] ToHashEntries(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties.Select(property => new HashEntry(property.Name, property.GetValue(obj).ToString())).ToArray();
        }

        /// <summary>
        ///  Deserialize form Redis format
        /// </summary>
        /// <param name="hashEntries"></param>
        /// <returns></returns>
        public static T ConvertFromRedis<T>(HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }

    }
}
