using SmtpServer;
using SmtpServer.Storage;
using Spamma.Api.Web.Infrastructure.Contracts;
using Spamma.Api.Web.Infrastructure.Contracts.MessageHandling;
using Spamma.Api.Web.Infrastructure.Contracts.SutWrappers;
using Spamma.Api.Web.Infrastructure.MessageHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IMessageStore, SpammaMessageStore>();
builder.Services.AddSingleton(
    provider =>
    {
        var options = new SmtpServerOptionsBuilder()
            .ServerName("SMTP Server")
            .Port(9025)
            .Build();

        return new SmtpServer.SmtpServer(options, provider.GetRequiredService<IServiceProvider>());
    });
builder.Services.AddHostedService<SmtpHostedService>();
builder.Services.AddSingleton<IMessageStoreProvider, LocalMessageStoreProvider>();
builder.Services.AddSingleton<IDirectoryWrapper, DirectoryWrapper>();

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();