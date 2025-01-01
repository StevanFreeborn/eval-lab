using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using Json.Schema;

namespace EvalLab.API.Evaluations;

record EvaluationDto(
  string Id,
  string Name,
  string Description,
  string TargetPipelineId,
  SuccessCriteria SuccessCriteria,
  DateTime CreatedDate,
  DateTime UpdatedDate
)
{
  private EvaluationDto(Evaluation evaluation) : this(
    evaluation.Id,
    evaluation.Name,
    evaluation.Description,
    evaluation.TargetPipelineId,
    evaluation.SuccessCriteria,
    evaluation.CreatedDate,
    evaluation.UpdatedDate
  )
  { }

  public bool TryValidate(out List<ValidationResult> results)
  {
    results = [];

    if (string.IsNullOrWhiteSpace(Name))
    {
      results.Add(new ValidationResult("Name is required", [nameof(Name)]));
    }

    if (string.IsNullOrWhiteSpace(TargetPipelineId))
    {
      results.Add(new ValidationResult($"{TargetPipelineId} is required", [nameof(TargetPipelineId)]));
    }

    if (SuccessCriteria is NullSuccessCriteria)
    {
      results.Add(new ValidationResult($"{nameof(SuccessCriteria)} is required", [nameof(SuccessCriteria)]));
    }

    if (SuccessCriteria is UnstructuredExactMatch unstructuredExactMatch)
    {
      if (string.IsNullOrWhiteSpace(unstructuredExactMatch.MatchValue))
      {
        var memberName = $"{nameof(SuccessCriteria)}.{nameof(unstructuredExactMatch.MatchValue)}";
        results.Add(new ValidationResult($"{memberName} is required", [memberName]));
      }
    }

    if (SuccessCriteria is UnstructuredPartialMatch unstructuredPartialMatch)
    {
      if (string.IsNullOrWhiteSpace(unstructuredPartialMatch.MatchValue))
      {
        var memberName = $"{nameof(SuccessCriteria)}.{nameof(unstructuredPartialMatch.MatchValue)}";
        results.Add(new ValidationResult($"{memberName} is required", [memberName]));
      }
    }

    if (SuccessCriteria is JsonMatch jsonMatch)
    {
      if (string.IsNullOrWhiteSpace(jsonMatch.Schema))
      {
        var memberName = $"{nameof(SuccessCriteria)}.{nameof(jsonMatch.Schema)}";
        results.Add(new ValidationResult($"{memberName} is required", [memberName]));
      }

      try
      {
        var schema = JsonSchema.FromText(jsonMatch.Schema);
      }
      catch (Exception ex) when (ex is JsonException)
      {
        var memberName = $"{nameof(SuccessCriteria)}.{nameof(jsonMatch.Schema)}";
        results.Add(new ValidationResult($"{memberName} is invalid", [memberName]));
      }
    }

    return results.Count is 0;
  }

  public static EvaluationDto From(Evaluation evaluation) => new(evaluation);

  public Evaluation ToEvaluation() => new()
  {
    Id = Id,
    Name = Name,
    Description = Description,
    TargetPipelineId = TargetPipelineId,
    SuccessCriteria = SuccessCriteria,
    CreatedDate = CreatedDate,
    UpdatedDate = UpdatedDate
  };
}