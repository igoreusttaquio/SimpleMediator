using NetDevPack.SimpleMediator.Core.Interfaces;
using System;

namespace Cadastro.Commands;

public class ExcluirClienteCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
