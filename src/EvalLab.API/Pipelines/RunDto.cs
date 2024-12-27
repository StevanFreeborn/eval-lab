namespace EvalLab.API.Pipelines;

record RunDto(string PipelineId, string Id, string Name, string Input, string Output, DateTime CreatedDate)
{
  public static RunDto From(Run run) => new(run.PipelineId, run.Id, run.Name, run.Input, run.Output, run.CreatedDate);
}