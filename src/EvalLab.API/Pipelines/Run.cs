using EvalLab.API.Data;

namespace EvalLab.API.Pipelines;

class Run : Entity
{
  public const string RunAttribute = "evallab.run";
  public const string RunIdPrefix = "run-";

  public string PipelineId { get; init; } = string.Empty;
  public string Name => $"Run {Id}";
  public string Input { get; init; } = string.Empty;
  public string Output { get; init; } = string.Empty;
}