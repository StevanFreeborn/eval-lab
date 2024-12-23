namespace EvalLab.API.Pipelines;

record Run(string Id, string Input, string Output)
{
  public DateTime CreatedDate { get; } = DateTime.UtcNow;
}