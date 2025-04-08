using System.Threading;
using System.Threading.Tasks;

namespace NetDevPack.SimpleMediator.Core.Interfaces
{

    public interface INotificationHandler<TNotification>
        where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken cancellationToken);
    }
}