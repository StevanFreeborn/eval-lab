using System.ComponentModel.DataAnnotations;

namespace EvalLab.API.Evaluations;

record AddEvaluationRunDto(
  string Input,
  int ExpectedProportion,
  int ConfidenceLevel,
  int MarginOfError,
  EvaluationDto Evaluation
)
{
  private readonly int[] _validConfidenceLevels = [80, 85, 90, 95, 99];
  private const int MinPercentage = 0;
  private const int MaxPercentage = 100;
  private static string InvalidPercentageMessage => $"Must be between {MinPercentage} and {MaxPercentage}";

  public bool TryValidate(out List<ValidationResult> results)
  {
    results = [];

    if (string.IsNullOrEmpty(Input))
    {
      results.Add(new ValidationResult("Input is required", [nameof(Input)]));
    }

    if (ExpectedProportion is < 0 or > MaxPercentage)
    {
      results.Add(new ValidationResult(InvalidPercentageMessage, [nameof(ExpectedProportion)]));
    }

    if (MarginOfError is < 0 or > MaxPercentage)
    {
      results.Add(new ValidationResult(InvalidPercentageMessage, [nameof(MarginOfError)]));
    }

    if (_validConfidenceLevels.Contains(ConfidenceLevel) is false)
    {
      results.Add(new ValidationResult($"Invalid confidence level. Must be one of {string.Join(",", _validConfidenceLevels)}.", [nameof(ConfidenceLevel)]));
    }

    if (Evaluation is null)
    {
      results.Add(new ValidationResult("Evaluation is required", [nameof(Evaluation)]));
    }

    if (Evaluation is not null && Evaluation.TryValidate(out var evaluationResults) is false)
    {
      foreach (var result in evaluationResults)
      {
        var modifiedMemberNames = result.MemberNames.Select(m => $"{nameof(Evaluation)}.{m}");
        results.Add(new ValidationResult(result.ErrorMessage, modifiedMemberNames));
      }
    }

    return results.Count is 0;
  }

  public EvaluationRun ToEvaluationRun() => new(
    Evaluation.Id,
    Input,
    Math.Round(ExpectedProportion / 100m, 2),
    Math.Round(ConfidenceLevel / 100m, 2),
    Math.Round(MarginOfError / 100m, 2)
  );
}