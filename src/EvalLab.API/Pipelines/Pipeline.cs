using EvalLab.API.Data;

namespace EvalLab.API.Pipelines;

class Pipeline : Entity
{
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public string Endpoint { get; init; } = string.Empty;
}