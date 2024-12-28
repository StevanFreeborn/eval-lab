namespace EvalLab.API.Evaluations;

record EvaluationDto(
  string Id,
  string Name,
  string Description,
  string Input,
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
    evaluation.Input,
    evaluation.TargetPipelineId,
    evaluation.SuccessCriteria,
    evaluation.CreatedDate,
    evaluation.UpdatedDate
  )
  { }

  public static EvaluationDto From(Evaluation evaluation) => new(evaluation);

  public Evaluation ToEvaluation() => new()
  {
    Id = Id,
    Name = Name,
    Description = Description,
    TargetPipelineId = TargetPipelineId,
    Input = Input,
    SuccessCriteria = SuccessCriteria,
    CreatedDate = CreatedDate,
    UpdatedDate = UpdatedDate
  };
}