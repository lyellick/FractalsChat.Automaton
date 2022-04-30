using FractalsChat.Automaton.Common.Context;
using FractalsChat.Automaton.WorkerService;
using FractalsChat.Automaton.WorkerService.Models;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        string connectionString = configuration.GetConnectionString("DefaultConnection");

        services
            .Configure<AppSettings>(options => hostContext.Configuration.GetSection("AppSettings").Bind(options))
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
