using Cadastro.Commands;
using NetDevPack.SimpleMediator.Core.Interfaces;
using System.Threading.Tasks;

namespace Cadastro;

public class ClienteService : IClienteService
{
    private readonly IMediator _mediator;

    public ClienteService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> CadastrarCliente(CadastrarClienteCommand cliente)
    {
        var resultado = await _mediator.Send(new CadastrarClienteCommand {Id = cliente.Id, Nome = cliente.Nome });
        return resultado;
    }

    public async Task<bool> ExcluirCliente(ExcluirClienteCommand cliente)
    {
        var resultado = await _mediator.Send(new ExcluirClienteCommand { Id = cliente.Id });
        return resultado;
    }
}