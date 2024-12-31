namespace EvalLab.API.Traces;

record TraceDto(
  string Id,
  string PipelineRunId,
  List<SpanDto> Spans,
  DateTime CreatedDate,
  DateTime UpdatedDate
)
{
  public string Name => $"Run {PipelineRunId}";
  public ulong Duration => GetDuration();
  public ulong Start => Spans.Min(span => span.Start);
  public ulong End => Spans.Max(span => span.End);
  public static TraceDto From(Trace trace) => new(
    trace.Id,
    trace.PipelineRunId,
    [.. trace.Spans.Select(SpanDto.From)],
    trace.CreatedDate,
    trace.UpdatedDate
  );

  private ulong GetDuration()
  {
    if (Spans.Count == 0)
    {
      return 0;
    }

    return End - Start;
  }
}

record SpanDto(string ParentId, string Id, string Name, ulong Start, ulong End, Dictionary<string, string> Attributes)
{
  public ulong Duration => End - Start;

  public static SpanDto From(TraceSpan span) => new(
    span.ParentId,
    span.Id,
    span.Name,
    span.Start,
    span.End,
    span.Attributes
  );
}