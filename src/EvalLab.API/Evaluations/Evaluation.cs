using EvalLab.API.Data;
using EvalLab.API.Pipelines;

namespace EvalLab.API.Evaluations;

class Evaluation : Entity
{
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public string TargetPipelineId { get; init; } = string.Empty;
  public SuccessCriteria SuccessCriteria { get; init; } = new NullSuccessCriteria();

  public async Task<(bool Passed, PipelineRun PipelineRun)> RunAsync(string input, Pipeline pipeline, HttpClient client)
  {
    var runResult = await pipeline.RunAsync(client, new(pipeline.Id, input));

    if (runResult.Failed)
    {
      throw new InvalidOperationException("Pipeline run failed");
    }

    var passed = SuccessCriteria.IsSatisfiedBy(runResult.Value.Output);

    return (passed, runResult.Value);
  }

  public async Task<(bool Passed, PipelineRun PipelineRun)> RunAsync(string input, IRepository<Pipeline> pipelineRepo, HttpClient client)
  {
    var pipeline = await pipelineRepo.GetAsync(FilterSpecification<Pipeline>.From(p => p.Id == TargetPipelineId));

    if (pipeline is null)
    {
      throw new InvalidOperationException("Pipeline not found");
    }

    return await RunAsync(input, pipeline, client);
  }
}

record TestResult(PipelineRun Run, bool Passed);