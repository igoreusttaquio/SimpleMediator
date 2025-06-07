using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetDevPack.SimpleMediator.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace NetDevPack.SimpleMediator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleMediator(
            this IServiceCollection services,
            ServiceLifetime lifetimeHandler = ServiceLifetime.Scoped,
            params object[] args)
        {
            var assemblies = ResolveAssemblies(args);

            // Registra o Mediator
            var descriptor = new ServiceDescriptor(typeof(IMediator), typeof(Mediator), lifetimeHandler);
            services.Add(descriptor);

            // Registra Handlers (Request e Notification)
            RegisterHandlers(services, assemblies, typeof(INotificationHandler<>), lifetimeHandler);
            RegisterHandlers(services, assemblies, typeof(IRequestHandler<,>), lifetimeHandler);

            // Registra behaviors (pipeline)
            //RegisterHandlers(services, assemblies, typeof(IPipelineBehavior<,>), lifetimeHandler);

            return services;
        }

        private static Assembly[] ResolveAssemblies(object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))
                    .ToArray();
            }

            if (args.All(a => a is Assembly))
                return args.Cast<Assembly>().ToArray();

            if (args.All(a => a is string))
            {
                var prefixes = args.Cast<string>().ToArray();
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a =>
                        !a.IsDynamic &&
                        !string.IsNullOrWhiteSpace(a.FullName) &&
                        prefixes.Any(p => a.FullName!.StartsWith(p)))
                    .ToArray();
            }

            throw new ArgumentException("Invalid parameters for AddSimpleMediator(). Use: no arguments, Assembly[], or prefix strings.");
        }

        private static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies, Type handlerInterface, ServiceLifetime serviceLifetime)
        {
            var types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .ToList();

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == handlerInterface);

                foreach (var iface in interfaces)
                {
                    var descriptor = new ServiceDescriptor(iface, type, serviceLifetime);

                    if (handlerInterface == typeof(IPipelineBehavior<,>))
                    {
                        // Para comportamentos de pipeline, registrar como enumerável, evita sobrescrever registros
                        services.TryAddEnumerable(descriptor);
                    }
                    else
                    {
                        // Para handlers normais, registro padrão
                        services.Add(descriptor);
                    }
                }
            }
        }

    }
}