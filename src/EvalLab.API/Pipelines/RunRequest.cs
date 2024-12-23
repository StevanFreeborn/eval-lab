using System.ComponentModel.DataAnnotations;

namespace EvalLab.API.Pipelines;

record RunRequest(string Input)
{
  public string Id { get; } = Guid.NewGuid().ToString();

  public bool TryValidate(out List<ValidationResult> results)
  {
    results = [];

    if (string.IsNullOrWhiteSpace(Input))
    {
      results.Add(new ValidationResult("Input is required", [nameof(Input)]));
    }

    return results.Count is 0;
  }
}