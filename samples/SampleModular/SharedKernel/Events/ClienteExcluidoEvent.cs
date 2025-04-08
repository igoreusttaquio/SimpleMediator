using NetDevPack.SimpleMediator.Core.Interfaces;
using System;

public record ClienteExcluidoEvent(Guid ClienteId) : INotification;

