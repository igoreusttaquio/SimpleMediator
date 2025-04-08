using Cadastro;
using Cadastro.Commands;
using Cadastro.Handlers;
using Crm.Handlers;
using Financeiro.Handlers;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator.Core.Extensions;
using NetDevPack.SimpleMediator.Core.Implementation;
using NetDevPack.SimpleMediator.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddScoped<IClienteService, ClienteService>();

// REGISTRO DO MEDIATOR

// Mais performatico
//builder.Services.AddSimpleMediator("Cadastro", "Crm", "Financeiro", "SharedKernel");

// Mais custoso
builder.Services.AddSimpleMediator();
//builder.Services.AddSimpleMediator(AppDomain.CurrentDomain.GetAssemblies());

// Mais trabalhoso
//services.AddSingleton<IMediator, Mediator>();
//services.AddTransient<IRequestHandler<CadastrarClienteCommand, string>, ClienteHandler>();
//services.AddTransient<IRequestHandler<ExcluirClienteCommand, bool>, ClienteHandler>();
//services.AddTransient<INotificationHandler<ClienteCadastradoEvent>, GestaoContaHandler>();
//services.AddTransient<INotificationHandler<ClienteCadastradoEvent>, NotificadorHandler>();
//services.AddTransient<INotificationHandler<ClienteExcluidoEvent>, GestaoContaHandler>();
//services.AddTransient<INotificationHandler<ClienteExcluidoEvent>, NotificadorHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/cadastrar", async ([FromBody] CadastrarClienteCommand command, IClienteService service) =>
{
    var resultado = await service.CadastrarCliente(command);
    return Results.Ok(resultado);
}).WithName("CadastrarCliente")
  .WithTags("Clientes");

app.MapPost("/excluir", async ([FromBody] ExcluirClienteCommand command, IClienteService service) =>
{
    var resultado = await service.ExcluirCliente(command);

    if (!resultado)
        return Results.BadRequest($"Problema ao excluir o cliente {command.Id}");

    return Results.Ok(command.Id);

}).WithName("ExcluirCliente")
  .WithTags("Clientes");

app.Run();