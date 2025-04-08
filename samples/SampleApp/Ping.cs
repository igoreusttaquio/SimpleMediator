using NetDevPack.SimpleMediator.Core.Interfaces;

public class Ping : IRequest<string>
{
    public string Message { get; set; } = "Ping!";
}