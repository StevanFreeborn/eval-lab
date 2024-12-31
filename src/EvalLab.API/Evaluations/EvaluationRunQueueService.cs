
namespace EvalLab.API.Evaluations;

class EvaluationRunQueueService(
  IEvaluationRunQueue queue,
  IEvaluationRunProcessor processor,
  ILogger<EvaluationRunQueueService> logger
) : BackgroundService
{
  private readonly IEvaluationRunQueue _queue = queue;
  private readonly IEvaluationRunProcessor _processor = processor;
  private readonly ILogger<EvaluationRunQueueService> _logger = logger;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (stoppingToken.IsCancellationRequested is false)
    {
      await ProcessItemAsync(stoppingToken);
    }
  }

  private async Task ProcessItemAsync(CancellationToken stoppingToken)
  {
    var run = await _queue.DequeueAsync();

    if (run is null)
    {
      return;
    }

    _ = Task.Run(
      async () =>
      {
        try
        {
          await _processor.ProcessAsync(run);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error processing evaluation run {EvaluationRunId}", run.Id);
        }
      },
      stoppingToken
    );

    _logger.LogInformation("Dequeued evaluation run {EvaluationRunId}", run.Id);
  }
}