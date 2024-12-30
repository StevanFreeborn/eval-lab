namespace EvalLab.API.Evaluations;

record EvaluationRunDto
{
  public string Id { get; init; } = string.Empty;
  public string Name => $"Run {Id}";
  public string EvaluationId { get; init; } = string.Empty;
  public int ExpectedProportion { get; init; }
  public int ConfidenceLevel { get; init; }
  public int MarginOfError { get; init; }
  public int SampleSize { get; init; }
  public string Status { get; init; } = string.Empty;
  public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedDate { get; init; } = DateTime.UtcNow;

  public static EvaluationRunDto From(EvaluationRun evaluationRun) => new()
  {
    Id = evaluationRun.Id,
    EvaluationId = evaluationRun.EvaluationId,
    ExpectedProportion = (int)(evaluationRun.ExpectedProportion * 100),
    ConfidenceLevel = (int)(evaluationRun.ConfidenceLevel * 100),
    MarginOfError = (int)(evaluationRun.MarginOfError * 100),
    SampleSize = evaluationRun.SampleSize,
    Status = evaluationRun.Status,
    CreatedDate = evaluationRun.CreatedDate,
    UpdatedDate = evaluationRun.UpdatedDate
  };
}