namespace EvalLab.API.Evaluations;

interface ITaskRunProcessor
{
  Task ProcessAsync(TestRunQueueItem item);
}

class TaskRunProcessor(
  IServiceScopeFactory scopeFactory,
  ILogger<TaskRunProcessor> logger
) : ITaskRunProcessor
{
  private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
  private readonly ILogger<TaskRunProcessor> _logger = logger;

  public async Task ProcessAsync(TestRunQueueItem item)
  {
    await Task.Delay(100);
    _logger.LogInformation("Processing sample {SampleNumber} for evaluation run {EvaluationRunId}", item.SampleNumber, item.EvaluationRunId);
  }
}