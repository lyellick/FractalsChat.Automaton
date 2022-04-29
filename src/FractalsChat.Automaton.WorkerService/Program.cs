using FractalsChat.Automaton.Common.Context;
using FractalsChat.Automaton.WorkerService;
using FractalsChat.Automaton.WorkerService.Models;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .Configure<AppSettings>(options => hostContext.Configuration.GetSection("AppSettings").Bind(options))
            .AddDbContext<FractalsChatContext>()
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
