using EvalLab.API.Data;

namespace EvalLab.API.Evaluations;

class Evaluation : Entity
{
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
}