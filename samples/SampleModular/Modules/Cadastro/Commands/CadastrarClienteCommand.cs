using NetDevPack.SimpleMediator.Core.Interfaces;
using System;

namespace Cadastro.Commands;

public class CadastrarClienteCommand : IRequest<string>
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
}
