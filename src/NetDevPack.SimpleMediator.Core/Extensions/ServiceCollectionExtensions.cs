using Microsoft.Extensions.DependencyInjection;
using NetDevPack.SimpleMediator.Core.Implementation;
using NetDevPack.SimpleMediator.Core.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace NetDevPack.SimpleMediator.Core.Extensions
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleMediator(
        this IServiceCollection services,
        params object[] args)
        {
            var assemblies = ResolveAssemblies(args);

            services.AddSingleton<IMediator, Mediator>();

            RegisterHandlers(services, assemblies, typeof(INotificationHandler<>));
            RegisterHandlers(services, assemblies, typeof(IRequestHandler<,>));

            return services;
        }

        private static Assembly[] ResolveAssemblies(object[] args)
        {
            // Nenhum argumento → retorna todos os carregados
            if (args == null || args.Length == 0)
            {
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))
                    .ToArray();
            }

            // Todos os args são Assembly
            if (args.All(a => a is Assembly))
                return args.Cast<Assembly>().ToArray();

            // Todos os args são string (prefixos)
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

            throw new ArgumentException("Parâmetros inválidos para AddSimpleMediator(). Use: nenhum argumento, Assembly[], ou strings de prefixo.");
        }


        private static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies, Type handlerInterface)
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
                    services.AddTransient(iface, type);
                }
            }
        }
    }
}