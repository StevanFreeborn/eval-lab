namespace EvalLab.API.Evaluations;

record EvaluationDto(
  string Id,
  string Name,
  string Description,
  DateTime CreatedDate,
  DateTime UpdatedDate
)
{
  private EvaluationDto(Evaluation evaluation) : this(
    evaluation.Id.ToString(),
    evaluation.Name,
    evaluation.Description,
    evaluation.CreatedDate,
    evaluation.UpdatedDate
  )
  { }

  public static EvaluationDto From(Evaluation evaluation) => new(evaluation);
}