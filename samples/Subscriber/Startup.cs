using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SamplesCommon.IntegrationEvents.Events;
using SamplesCommon.IntegrationEvents.Handlers;
using Serilog;
using ZC.MQBus.Base;
using ZC.MQBus.Base.Options;
using ZC.MQBus.Rabbit.Configuration;

namespace SubscriberRPC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var rabbitMqOptionsSection = Configuration.GetSection("RabbitMq");

            var rabbitMqOptions = rabbitMqOptionsSection.Get<RabbitMqOptions>();

            services.Configure<RabbitMqOptions>(rabbitMqOptionsSection);
            
            services.AddMessageBus();
            services.AddTransient<PingEventHandler>();
            services.AddTransient<RPCPingRequestHandler>();
            services.AddTransient<PingCommandHandler>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Subscriber test API",
                    Version = "v1",
                });
            });

            var serilogLogger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.File("./logs/log.txt",
                            rollingInterval: RollingInterval.Day,
                            rollOnFileSizeLimit: true)
                        .CreateLogger();

            services.AddLogging(x =>
            {
                x.SetMinimumLevel(LogLevel.Information);
                x.AddSerilog(logger: serilogLogger, dispose: true);
                x.AddConsole();
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Event Bus
            var eventBus = app.ApplicationServices.GetRequiredService<IMessageBus>();

            eventBus.Subscribe<PingEvent, PingEventHandler>();
            eventBus.SubscribeRPC<RPCPingRequest, RPCPingRequestHandler, RPCPingResponse>();
            eventBus.SubscribeCommand<PingCommand, PingCommandHandler>();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "SubscriberRPC API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
