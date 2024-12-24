namespace EvalLab.API.Pipelines;

record Run(string Id, string Input, string Output)
{
  public const string RunAttribute = "evallab.run";
  public const string RunIdPrefix = "run-";
  public DateTime CreatedDate { get; } = DateTime.UtcNow;
}