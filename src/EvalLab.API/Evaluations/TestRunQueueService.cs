
namespace EvalLab.API.Evaluations;

class TestRunQueueService(
  ITestRunQueue queue,
  ITaskRunProcessor processor,
  ILogger<TestRunQueueService> logger
) : BackgroundService
{
  private readonly ITestRunQueue _queue = queue;
  private readonly ITaskRunProcessor _processor = processor;
  private readonly ILogger<TestRunQueueService> _logger = logger;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (stoppingToken.IsCancellationRequested is false)
    {
      await ProcessItemAsync(stoppingToken);
    }
  }

  private async Task ProcessItemAsync(CancellationToken stoppingToken)
  {
    var item = await _queue.DequeueAsync();

    if (item is null)
    {
      return;
    }

    _ = Task.Run(
      async () =>
      {
        try
        {
          await _processor.ProcessAsync(item);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error processing sample {SampleNumber} for evaluation run {EvaluationRunId}", item.SampleNumber, item.EvaluationRunId);
        }
      },
      stoppingToken
    );

    _logger.LogInformation("Dequeued sample {SampleNumber} for evaluation run {EvaluationRunId}", item.SampleNumber, item.EvaluationRunId);
  }
}