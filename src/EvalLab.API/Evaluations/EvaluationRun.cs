using EvalLab.API.Data;

namespace EvalLab.API.Evaluations;

class EvaluationRun : Entity
{
  private readonly Dictionary<decimal, decimal> _zScores = new()
  {
    { 0.80m, 1.28m },
    { 0.85m, 1.44m },
    { 0.90m, 1.64m },
    { 0.95m, 1.96m },
    { 0.99m, 2.58m }
  };

  public string EvaluationId { get; init; } = string.Empty;
  public string Input { get; init; }
  public decimal ExpectedProportion { get; init; }
  public decimal ConfidenceLevel { get; init; }
  public decimal MarginOfError { get; init; }
  public int SampleSize { get; init; }
  public List<TestRun> TestRuns { get; init; } = [];
  public string Status => TestRuns.Count == SampleSize ? "Completed" : "Running";
  public decimal? SuccessRate => Status == "Completed" ? Math.Round((decimal)TestRuns.Count(tr => tr.Passed) / SampleSize, 2) : null;

  public EvaluationRun(
    string evaluationId,
    string input,
    decimal expectedProportion,
    decimal confidenceLevel,
    decimal marginOfError
  )
  {
    EvaluationId = evaluationId;
    Input = input;
    ExpectedProportion = expectedProportion;
    ConfidenceLevel = confidenceLevel;
    MarginOfError = marginOfError;
    SampleSize = CalculateSampleSize();
  }

  /// <summary>
  /// Calculate the sample size required for the evaluation run using Cochran's formula.
  /// </summary>
  private int CalculateSampleSize()
  {
    var z = _zScores[ConfidenceLevel];
    var p = ExpectedProportion;
    var e = MarginOfError;
    var q = 1 - p;

    var numerator = z * z * p * q;
    var denominator = e * e;

    var result = (int)Math.Ceiling((double)(numerator / denominator));

    return Math.Max(result, 1);
  }
}

record TestRun
{
  public string PipelineRunId { get; init; } = string.Empty;
  public bool Passed { get; init; }
}