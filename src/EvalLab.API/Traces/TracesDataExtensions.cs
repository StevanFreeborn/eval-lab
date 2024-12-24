using EvalLab.API.Pipelines;

using OpenTelemetry.Proto.Trace.V1;

namespace EvalLab.API.Traces;

static class TracesDataExtensions
{
  public static Trace ToTrace(this TracesData data)
  {
    var spans = data.ResourceSpans
      .SelectMany(rs => rs.ScopeSpans)
      .First(ss => ss.Spans.Any(s => s.Attributes.Any(a => a.Key is Run.RunAttribute)))
      .Spans
      .ToList();

    var runId = spans
      .SelectMany(s => s.Attributes)
      .First(a => a.Key is Run.RunAttribute)
      .Value
      .StringValue;

    var id = runId.Split(Run.RunIdPrefix)[1];

    return new() { RunId = id, Spans = spans };
  }
}