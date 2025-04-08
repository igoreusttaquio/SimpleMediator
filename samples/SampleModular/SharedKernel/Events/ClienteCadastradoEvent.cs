using NetDevPack.SimpleMediator.Core.Interfaces;
using System;

public record ClienteCadastradoEvent(Guid ClienteId) : INotification;

