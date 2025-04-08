using Cadastro.Commands;
using NetDevPack.SimpleMediator.Core.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Cadastro.Handlers;

public class ClienteHandler : 
    IRequestHandler<CadastrarClienteCommand, string>,
    IRequestHandler<ExcluirClienteCommand, bool>
{
    private readonly IMediator _mediator;

    public ClienteHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> Handle(CadastrarClienteCommand request, CancellationToken cancellationToken)
    {
        // simula persistência (poderia usar um repositório aqui)
        request.Id = Guid.NewGuid();

        // dispara evento após criação
        await _mediator.Publish(new ClienteCadastradoEvent(request.Id), cancellationToken);

        return $"Cliente {request.Nome} cadastrado com sucesso.";
    }

    public async Task<bool> Handle(ExcluirClienteCommand request, CancellationToken cancellationToken)
    {
        // dispara evento após criação
        await _mediator.Publish(new ClienteExcluidoEvent(request.Id), cancellationToken);

        return true;
    }
}