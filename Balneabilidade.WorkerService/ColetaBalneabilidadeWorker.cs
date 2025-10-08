using Balneabilidade.Jobs;

namespace Balneabilidade.WorkerService;

public class ColetaBalneabilidadeWorker : BackgroundService
{
    private readonly ILogger<ColetaBalneabilidadeWorker> _logger;
    private readonly AtualizarDadosBalneabilidadeJob _job;

    public ColetaBalneabilidadeWorker(ILogger<ColetaBalneabilidadeWorker> logger, AtualizarDadosBalneabilidadeJob job)
    {
        _logger = logger;
        _job = job;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado.");

        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

        // Roda a primeira vez imediatamente ao iniciar o serviço
        _logger.LogInformation("Executando a tarefa pela primeira vez...");
        await _job.ExecutarAsync();

        // Entra em um loop que espera o timer
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Executando a tarefa agendada...");
            await _job.ExecutarAsync();
        }
    }
}