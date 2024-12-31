using System.ComponentModel.DataAnnotations;

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
      results.Add(new ValidationResult("TargetPipelineId is required", [nameof(TargetPipelineId)]));
    }

    if (SuccessCriteria is NullSuccessCriteria)
    {
      results.Add(new ValidationResult("SuccessCriteria is required", [nameof(SuccessCriteria)]));
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