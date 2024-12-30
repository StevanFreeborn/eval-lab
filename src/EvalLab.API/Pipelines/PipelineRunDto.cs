namespace EvalLab.API.Pipelines;

record PipelineRunDto(
  string PipelineId,
  string Id,
  string Name,
  string Input,
  string Output,
  DateTime CreatedDate
)
{
  public static PipelineRunDto From(PipelineRun run) => new(
    run.PipelineId,
    run.Id,
    run.Name,
    run.Input,
    run.Output,
    run.CreatedDate
  );
}