using Autofac;
using Essensoft.AspNetCore.Payment.WeChatPay;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentAPI.Extensions;
using PaymentAPI.Filters;
using PaymentAPI.Helpers;
using PaymentAPI.Middlewares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PaymentAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton(new Appsettings(Configuration));
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddCorsSetup();
            services.AddSwaggerSetup();
            services.AddAuthorizationSetup();
            services.AddHttpContextSetup();
            services.AddRedisCacheSetup();
            services.AddWeChatPay();
            services.Configure<WeChatPayOptions>(Configuration.GetSection("WeChatPay"));
            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionsFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwaggerMildd(() => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("PaymentAPI.index.html"));

            app.UseCors("AllowCorsPolicys");

            var option = new RewriteOptions();
            option.AddRedirect("^$", "spec");
            app.UseRewriter(option);

            app.UseWelcomePage(new WelcomePageOptions
            {
                Path = "/welcome"
            });


            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                RequestPath = new PathString("/assets")
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
