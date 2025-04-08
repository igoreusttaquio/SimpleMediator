using NetDevPack.SimpleMediator.Core.Extensions;
using NetDevPack.SimpleMediator.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSimpleMediator(typeof(Program).Assembly);

var app = builder.Build();

app.MapGet("/", async (IMediator mediator) =>
{
    var result = await mediator.Send(new Ping { Message = "Hello Mediator" });
    return Results.Ok(result);
});

app.Run();