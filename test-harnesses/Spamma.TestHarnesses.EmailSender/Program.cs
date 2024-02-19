using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tynamix.ObjectFiller;
using Email = Spamma.TestHarnesses.EmailSender.Email;

var emailFiller = new Filler<Email>();
emailFiller.Setup()
    .OnProperty(e => e.To).Use(new EmailAddresses())
    .OnProperty(e => e.From).Use(new EmailAddresses())
    .OnProperty(e => e.Subject).Use(new MnemonicString())
    .OnProperty(e => e.Body).Use(new Lipsum());
var email = emailFiller.Create();

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services
            .AddFluentEmail(email.From)
            .AddSmtpSender("localhost", 9025);
    })
    .ConfigureLogging((_, logging) =>
    {
        logging.ClearProviders();
        logging.AddSimpleConsole(options => options.IncludeScopes = true);
    })
    .Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var fluentEmail = host.Services.GetRequiredService<IFluentEmail>();
var response = await fluentEmail.To(email.To)
    .Body(email.Body)
    .Subject(email.Subject)
    .SendAsync();
logger.LogInformation("Response from smtp call: {@response}", response.Successful);