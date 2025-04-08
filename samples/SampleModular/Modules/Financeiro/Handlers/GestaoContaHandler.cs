using NetDevPack.SimpleMediator.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Financeiro.Handlers;

public class GestaoContaHandler : 
    INotificationHandler<ClienteCadastradoEvent>,
    INotificationHandler<ClienteExcluidoEvent>
{
    public Task Handle(ClienteCadastradoEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Financeiro] Criando conta para cliente {notification.ClienteId}");
        return Task.CompletedTask;
    }

    public Task Handle(ClienteExcluidoEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Financeiro] Excluindo conta do cliente {notification.ClienteId}");
        return Task.CompletedTask;
    }
}