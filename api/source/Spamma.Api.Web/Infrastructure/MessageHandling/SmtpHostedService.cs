namespace Spamma.Api.Web.Infrastructure.MessageHandling
{
    public class SmtpHostedService(SmtpServer.SmtpServer smtpServer)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await smtpServer.StartAsync(stoppingToken);
        }
    }
}