namespace EvalLab.API.Pipelines;

record RunDto(string Id, string Input, string Output, DateTime CreatedDate)
{
  public static RunDto From(Run run) => new(run.Id, run.Input, run.Output, run.CreatedDate);
}