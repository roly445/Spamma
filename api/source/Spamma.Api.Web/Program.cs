using SmtpServer;
using SmtpServer.Storage;
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

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();