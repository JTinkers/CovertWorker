using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace CovertWorker
{
    public class Initializer
    {
        public Initializer(IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                .AddJsonFile("./config.json", false, true)
                .AddJsonFile($"./config.{env}.json", true, true)
                .Build();

            var serviceConfig = config.GetSection("Service")
                .Get<ServiceConfig>();

            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton(serviceConfig);
            services.AddHostedService<Worker>();
        }
    }
}
