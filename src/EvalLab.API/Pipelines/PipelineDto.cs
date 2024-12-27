namespace EvalLab.API.Pipelines;

record PipelineDto(
  string Id,
  string Name,
  string Description,
  string Endpoint,
  DateTime CreatedDate,
  DateTime UpdatedDate
)
{
  private PipelineDto(Pipeline pipeline) : this(
    pipeline.Id.ToString(),
    pipeline.Name,
    pipeline.Description,
    pipeline.Endpoint,
    pipeline.CreatedDate,
    pipeline.UpdatedDate
  )
  { }

  public static PipelineDto From(Pipeline pipeline) => new(pipeline);
}

record PipelineWithRunsDto(
  string Id,
  string Name,
  string Description,
  string Endpoint,
  DateTime CreatedDate,
  DateTime UpdatedDate,
  List<RunDto> Runs
)
{
  private PipelineWithRunsDto(Pipeline pipeline) : this(
    pipeline.Id.ToString(),
    pipeline.Name,
    pipeline.Description,
    pipeline.Endpoint,
    pipeline.CreatedDate,
    pipeline.UpdatedDate,
    [.. pipeline.Runs.Select(RunDto.From)]
  )
  { }

  public static PipelineWithRunsDto From(Pipeline pipeline) => new(pipeline);
}