using FractalsChat.Automaton.WorkerService;
using FractalsChat.Automaton.WorkerService.Models;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .Configure<AppSettings>(options => hostContext.Configuration.GetSection("AppSettings").Bind(options))
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
