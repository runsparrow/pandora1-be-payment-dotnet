using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PaymentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 8005, listenOptions =>
                        {
                            listenOptions.UseHttps("t-pic-cn-iis-1128142825.pfx", "t-pic.cn");
                        });
                    })
                    .UseStartup<Startup>()
                     .ConfigureLogging((hostingContext, builder) =>
                     {
                         builder.AddFilter("System", LogLevel.Error);
                         builder.AddFilter("Microsoft", LogLevel.Error);
                         var path = Path.Combine(Directory.GetCurrentDirectory(), "log4net.config");
                         builder.AddLog4Net(path);
                     }); ;
                });
    }
}
