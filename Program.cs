using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace CovertWorker
{
    public class Program
    {
        static Task Main(string[] args) 
            => CreateHostBuilder(args)
                .Build()
                .RunAsync();

        static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .UseWindowsService(o =>
                {
                    o.ServiceName = "JTinkers' Covert Worker - File Converter";
                })
                .ConfigureServices((c) =>
                {
                    c.AddSingleton(new Initializer(c));
                });
    }
}
