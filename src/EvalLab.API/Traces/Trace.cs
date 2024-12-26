using EvalLab.API.Data;

namespace EvalLab.API.Traces;

class Trace : Entity
{
  public string RunId { get; init; } = string.Empty;
  public List<TraceSpan> Spans { get; init; } = null!;
}

class TraceSpan
{
  public string ParentId { get; init; } = string.Empty;
  public string Id { get; init; } = string.Empty;
  public string Name { get; init; } = string.Empty;
  public ulong Start { get; init; }
  public ulong End { get; init; }
  public Dictionary<string, string> Attributes { get; init; } = null!;
}