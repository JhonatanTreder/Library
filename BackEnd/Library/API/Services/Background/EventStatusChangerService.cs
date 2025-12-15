using API.Repositories.Interfaces;

public class EventStatusChangerService : BackgroundService
{
    private readonly ILogger<EventStatusChangerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(2);
    private readonly int _retryDelaySeconds = 60;

    public EventStatusChangerService(ILogger<EventStatusChangerService> logger,
                                     IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine();
        Console.WriteLine("-----------------------------------------------------------------------------------------");
        _logger.LogInformation("(1/3) Serviço de alteração de status dos eventos iniciado.");
        Console.WriteLine();

        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                _logger.LogInformation("(2/3) Iniciando verificação de status dos eventos.");
                Console.WriteLine("-----------------------------------------------------------------------------------------");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

                    var updated = await eventRepository.UpdateEventsStatusAsync();
                    var archived = await eventRepository.ArchiveOldEventsAsync();

                    if (updated > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("-----------------------------------------------------------------------------------------");
                        _logger.LogInformation(
                            $"(3/3) Operação de atualização finalizada: {updated} eventos tiveram seus status atualizados.");
                        Console.WriteLine("-----------------------------------------------------------------------------------------");
                    }

                    if (archived > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("-----------------------------------------------------------------------------------------");
                        _logger.LogInformation(
                            $"(3/3) Operação de arquivar finalizada: {archived} eventos foram arquivados.");
                        Console.WriteLine("-----------------------------------------------------------------------------------------");
                    }

                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("-----------------------------------------------------------------------------------------");
                        _logger.LogInformation("(3/3) Nenhum evento precisou ser atualizado ou arquivado.");
                        Console.WriteLine("-----------------------------------------------------------------------------------------");
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Erro ao processar status dos eventos. Tentando novamente em {_retryDelaySeconds} segundos.");

                await Task.Delay(TimeSpan.FromSeconds(_retryDelaySeconds), stoppingToken);
                continue;
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("O serviço de alteração de status de eventos está sendo parado...");

        await base.StopAsync(cancellationToken);
    }
}