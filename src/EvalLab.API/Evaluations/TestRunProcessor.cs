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
    // TODO: Implement the processing logic here
    // retrieve the evaluation run
    // retrieve the evaluation
    // retrieve the target pipeline
    // provide evaluation input to the pipeline
    // run the pipeline
    // apply evaluation success criteria to the pipeline output
    // add a test run to the evaluation run
    // save the evaluation run
    await Task.Delay(100);
    _logger.LogInformation("Processing sample {SampleNumber} for evaluation run {EvaluationRunId}", item.SampleNumber, item.EvaluationRunId);
  }
}