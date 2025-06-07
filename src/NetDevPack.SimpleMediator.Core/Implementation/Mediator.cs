using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.SimpleMediator.Interfaces;

namespace NetDevPack.SimpleMediator
{

    public class Mediator : IMediator
    {
        private readonly IServiceProvider _provider;

        public Mediator(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
            var handler = _provider.GetService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"Handler not found for {requestType.Name}");

            var handlerMethod = handlerType.GetMethod("Handle");
            if (handlerMethod == null)
                throw new InvalidOperationException($"Handler method not found for {requestType.Name}");

            // Pega pipeline behaviors (reverso para encadeamento correto)
            var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
            var behaviors = _provider.GetServices(behaviorType).Reverse().ToList();

            // Delegate que chama o handler final, via Delegate.CreateDelegate
            var handlerDelegate = CreateHandlerDelegate<TResponse>(handler, handlerMethod, request);

            // Encadeia os behaviors sem usar dynamic, via reflexão
            foreach (var behavior in behaviors)
            {
                var next = handlerDelegate;

                // Cria um delegate que chama Handle do behavior via reflexão
                handlerDelegate = ct =>
                {
                    var handleMethod = behavior?.GetType().GetMethod("Handle");
                    if (handleMethod == null)
                        throw new InvalidOperationException("Behavior handle method not found");

                    // Invoca Handle( IRequest<TResponse>, CancellationToken, RequestHandlerDelegate<TResponse> )
                    var task = (Task<TResponse>)handleMethod.Invoke(behavior, new object[] { request, ct, next })!;
                    return task;
                };
            }

            // Executa o pipeline completo
            return await handlerDelegate(cancellationToken);
        }

        // Helper para criar o delegate para o handler
        private static RequestHandlerDelegate<TResponse> CreateHandlerDelegate<TResponse>(
            object handler,
            System.Reflection.MethodInfo handlerMethod,
            IRequest<TResponse> request)
        {
            return ct => (Task<TResponse>)handlerMethod.Invoke(handler, new object[] { request, ct })!;
        }

        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            var handlers = _provider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                await (Task)handlerType
                    .GetMethod("Handle")!
                    .Invoke(handler, new object[] { notification, cancellationToken })!;
            }
        }
    }
}