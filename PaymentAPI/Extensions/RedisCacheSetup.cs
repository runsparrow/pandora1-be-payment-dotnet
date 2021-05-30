using Microsoft.Extensions.DependencyInjection;
using PaymentAPI.Helpers;
using PaymentAPI.Helpers.Redis;
using StackExchange.Redis;
using System;

namespace PaymentAPI.Extensions
{
    public static class RedisCacheSetup
    {
        public static void AddRedisCacheSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IRedisCacheManager, RedisCacheManager>();

            services.AddSingleton<ConnectionMultiplexer>(sp =>
               {
                   string redisConfiguration = Appsettings.app(new string[] { "DataBases","Redis", "ConnectionString" });

                   var configuration = ConfigurationOptions.Parse(redisConfiguration, true);

                   configuration.ResolveDns = true;

                   return ConnectionMultiplexer.Connect(configuration);
               });

        }
    }
}
