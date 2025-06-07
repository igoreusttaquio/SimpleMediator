using System.Threading.Tasks;
using System.Threading;

namespace NetDevPack.SimpleMediator.Interfaces
{
    public interface IPipelineBehavior<TRequest, TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next);
    }

    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);
}
