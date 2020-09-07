﻿using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace SampleFrameworkSubscriberCMD.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterOptions(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(OptionsManager<>))
              .As(typeof(IOptions<>))
              .SingleInstance();
            builder.RegisterGeneric(typeof(OptionsMonitor<>))
              .As(typeof(IOptionsMonitor<>))
              .SingleInstance();
            //builder.RegisterGeneric(typeof(OptionsSnapshot<>))
            //  .As(typeof(IOptionsSnapshot<>))
            //  .InstancePerLifetimeScope();
        }

        public static void RegisterConfigurationOptions<TOptions>(this ContainerBuilder builder, IConfiguration config) where TOptions : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            builder.RegisterInstance(new ConfigurationChangeTokenSource<TOptions>(config))
                .As<IOptionsChangeTokenSource<TOptions>>()
                .SingleInstance();

            builder.RegisterInstance(new ConfigureFromConfigurationOptions<TOptions>(config))
                .As<IConfigureOptions<TOptions>>()
                .SingleInstance();
        }

        public static void Configure<TOptions>(this ContainerBuilder builder, Action<TOptions> configureOptions) where TOptions : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            builder.RegisterInstance(new ConfigureOptions<TOptions>(configureOptions))
              .As<IConfigureOptions<TOptions>>()
              .SingleInstance();
        }
    }
}
