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

        public readonly IIRCNetworkAutomationService _automation;

        public Worker(IOptions<AppSettings> options, ILogger<Worker> logger, IServiceProvider services)
        {
            using IServiceScope scope = services.CreateScope();

            _automation = scope.ServiceProvider.GetRequiredService<IIRCNetworkAutomationService>();
            _settings = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}