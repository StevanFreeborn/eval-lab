using EvalLab.API.Data;
using EvalLab.API.Pipelines;

namespace EvalLab.API.Evaluations;

interface IEvaluationRunProcessor
{
  Task ProcessAsync(EvaluationRun run);
}

class EvaluationRunProcessor(
  IServiceScopeFactory scopeFactory,
  ILogger<EvaluationRunProcessor> logger
) : IEvaluationRunProcessor
{
  private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
  private readonly ILogger<EvaluationRunProcessor> _logger = logger;

  public async Task ProcessAsync(EvaluationRun run)
  {
    using var scope = _scopeFactory.CreateAsyncScope();
    var evaluationRunRepository = scope.ServiceProvider.GetRequiredService<IRepository<EvaluationRun>>();
    var evaluationRepository = scope.ServiceProvider.GetRequiredService<IRepository<Evaluation>>();
    var pipelineRepository = scope.ServiceProvider.GetRequiredService<IRepository<Pipeline>>();
    var pipelineRunRepository = scope.ServiceProvider.GetRequiredService<IRepository<PipelineRun>>();
    var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

    _logger.LogInformation("Processing evaluation run {EvaluationRunId}", run.Id);

    var evaluation = await evaluationRepository.GetAsync(FilterSpecification<Evaluation>.From(e => e.Id == run.EvaluationId));

    if (evaluation is null)
    {
      _logger.LogWarning("Evaluation {EvaluationId} not found. Skipping evaluation run {EvaluationRunId}", run.EvaluationId, run.Id);
      return;
    }

    var pipeline = await pipelineRepository.GetAsync(FilterSpecification<Pipeline>.From(p => p.Id == evaluation.TargetPipelineId));

    foreach (var num in Enumerable.Range(0, run.SampleSize))
    {
      var pipelineRunResult = await pipeline.RunAsync(httpClient, new(pipeline.Id, run.Input));

      var testRun = new TestRun
      {
        PipelineRunId = string.Empty,
        Passed = false
      };

      if (pipelineRunResult.Succeeded)
      {
        await pipelineRunRepository.CreateAsync(pipelineRunResult.Value);

        testRun = new TestRun
        {
          PipelineRunId = pipelineRunResult.Value.Id,
          Passed = true
        };
      }

      run.TestRuns.Add(testRun);
    }

    await evaluationRunRepository.UpdateAsync(
      FilterSpecification<EvaluationRun>.From(er => er.Id == run.Id),
      run
    );

    _logger.LogInformation("Evaluation run {EvaluationRunId} processed", run.Id);
  }
}