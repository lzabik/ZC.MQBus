using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleFrameworkSubscriberCMD.Extensions;
using SamplesCommon.IntegrationEvents.Events;
using SamplesCommon.IntegrationEvents.Handlers;
using System;
using System.IO;
using ZC.MQBus.Base;
using ZC.MQBus.Base.Options;
using ZC.MQBus.Rabbit.Configuration;

namespace SampleFrameworkSubscriberCMD
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static IContainer Container { get; private set; }

        public static IConfiguration Configuration { get; private set; }

        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = configBuilder.Build();

            var services = new ServiceCollection();

            ConfigureServices(services);

            var builder = new ContainerBuilder();

            RegisterExtras(builder);
            builder.Populate(services);

            Container = builder.Build();

            ServiceProvider = new AutofacServiceProvider(Container);

            SubscribeToEvents();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void RegisterExtras(ContainerBuilder builder)
        {
            builder.RegisterOptions();
            builder.RegisterConfigurationOptions<RabbitMqOptions>(Configuration.GetSection("RabbitMq"));

            builder.RegisterInstance(new LoggerFactory())
               .As<ILoggerFactory>();

            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();
        }

        private static void SubscribeToEvents()
        {
            var messageBus = ServiceProvider.GetRequiredService<IMessageBus>();

            messageBus.Subscribe<PingEvent, PingEventHandler>();
            messageBus.SubscribeRPC<RPCPingRequest, RPCPingRequestHandler, RPCPingResponse>();
            messageBus.SubscribeCommand<PingCommand, PingCommandHandler>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddMessageBus();

            services.AddTransient<PingEventHandler>();
            services.AddTransient<RPCPingRequestHandler>();
            services.AddTransient<PingCommandHandler>();
        }
    }
}
