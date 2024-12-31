using System.ComponentModel.DataAnnotations;

namespace EvalLab.API.Evaluations;

record AddEvaluationDto(string Name, string Description)
{
  public bool TryValidate(out List<ValidationResult> results)
  {
    results = [];

    if (string.IsNullOrWhiteSpace(Name))
    {
      results.Add(new ValidationResult("Name is required", [nameof(Name)]));
    }

    return results.Count is 0;
  }

  public Evaluation ToEvaluation() => new() { Name = Name, Description = Description };
}