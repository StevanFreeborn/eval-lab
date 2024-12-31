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
    pipeline.Id,
    pipeline.Name,
    pipeline.Description,
    pipeline.Endpoint,
    pipeline.CreatedDate,
    pipeline.UpdatedDate
  )
  { }

  public static PipelineDto From(Pipeline pipeline) => new(pipeline);
}