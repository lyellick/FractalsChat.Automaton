using FractalsChat.Automaton.Common.Context;
using FractalsChat.Automaton.Common.Services;
using FractalsChat.Automaton.WorkerService.Models;
using Microsoft.Extensions.Options;

namespace FractalsChat.Automaton.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly AppSettings _settings;

        public readonly IServiceProvider _services;

        public Worker(IOptions<AppSettings> options, ILogger<Worker> logger, IServiceProvider services)
        {
            _services = services;
            _settings = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = _services.CreateScope();

            FractalsChatContext context = scope.ServiceProvider.GetRequiredService<FractalsChatContext>();

            await context.Database.EnsureCreatedAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                
            }
        }
    }
}