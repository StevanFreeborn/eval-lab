using System.ComponentModel.DataAnnotations;

namespace EvalLab.API.Evaluations;

record TestEvaluationDto(string Input, EvaluationDto Evaluation)
{
  public bool TryValidate(out List<ValidationResult> results)
  {
    results = [];

    if (string.IsNullOrWhiteSpace(Input))
    {
      results.Add(new ValidationResult("Input is required", [nameof(Input)]));
    }

    if (Evaluation is null)
    {
      results.Add(new ValidationResult("Evaluation is required", [nameof(Evaluation)]));
    }

    return results.Count is 0;
  }
}