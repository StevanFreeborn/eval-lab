using EvalLab.API.Data;
using EvalLab.API.Pipelines;

namespace EvalLab.API.Evaluations;

class Evaluation : Entity
{
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public string TargetPipelineId { get; init; } = string.Empty;
  public string Input { get; init; } = string.Empty;
  public SuccessCriteria SuccessCriteria { get; init; } = new NullSuccessCriteria();

  public async Task<TestResult> RunAsync(Pipeline pipeline, HttpClient client)
  {
    var runResult = await pipeline.RunAsync(client, new(pipeline.Id, Input));

    if (runResult.Failed)
    {
      throw new InvalidOperationException("Pipeline run failed");
    }

    var passed = SuccessCriteria.IsSatisfiedBy(runResult.Value.Output);

    return new TestResult(runResult.Value, passed);
  }

  public async Task<TestResult> RunAsync(IRepository<Pipeline> pipelineRepo, HttpClient client)
  {
    var pipeline = await pipelineRepo.GetAsync(FilterSpecification<Pipeline>.From(p => p.Id == TargetPipelineId));

    if (pipeline is null)
    {
      throw new InvalidOperationException("Pipeline not found");
    }

    return await RunAsync(pipeline, client);
  }
}

record TestResult(Run Run, bool Passed);