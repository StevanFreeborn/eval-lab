using EvalLab.API.Pipelines;

record TestRunDto(PipelineRunDto PipelineRun, bool Passed)
{
  public static TestRunDto From(PipelineRun run, bool passed) => new(PipelineRunDto.From(run), passed);
}