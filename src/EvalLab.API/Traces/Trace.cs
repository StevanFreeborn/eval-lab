using EvalLab.API.Data;

using OpenTelemetry.Proto.Trace.V1;

namespace EvalLab.API.Traces;

class Trace : Entity
{
  public string RunId { get; init; } = string.Empty;
  public List<Span> Spans { get; init; } = null!;
}