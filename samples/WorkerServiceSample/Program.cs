using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SamplesCommon.IntegrationEvents.Events;
using SamplesCommon.IntegrationEvents.Handlers;
using ZC.MQBus.Base;
using ZC.MQBus.Base.Options;
using ZC.MQBus.Rabbit.Configuration;

namespace WorkerServiceSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var rabbitMqOptionsSection = hostContext.Configuration.GetSection("RabbitMq");
                    var rabbitMqOptions = rabbitMqOptionsSection.Get<RabbitMqOptions>();
                    services.Configure<RabbitMqOptions>(rabbitMqOptionsSection);
                    
                    services.AddMessageBus();
                    services.AddTransient<PingEventHandler>();
                    services.AddTransient<RPCPingRequestHandler>();

                    services.AddHostedService<Worker>();
                });
    }
}
