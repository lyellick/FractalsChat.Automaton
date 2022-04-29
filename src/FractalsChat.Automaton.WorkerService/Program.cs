using FractalsChat.Automaton.Common.Context;
using FractalsChat.Automaton.Common.Services;
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
            .AddDbContext<FractalsChatContext>(options => options.UseLazyLoadingProxies().UseSqlite(connectionString))
            .AddScoped<IIRCNetworkConnectionService, IRCNetworkConnectionService>()
            .AddHostedService<Worker>();
    })
    .Build();

using IServiceScope scope = host.Services.CreateScope();

FractalsChatContext context = scope.ServiceProvider.GetRequiredService<FractalsChatContext>();

await context.Database.EnsureCreatedAsync();

await host.RunAsync();
