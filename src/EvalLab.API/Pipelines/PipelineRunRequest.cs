using System.ComponentModel.DataAnnotations;

namespace EvalLab.API.Pipelines;

record PipelineRunRequest(string PipelineId, string Input)
{
  public string Id { get; init; } = Guid.NewGuid().ToString();

  public bool TryValidate(out List<ValidationResult> results)
  {
    results = [];

    if (string.IsNullOrWhiteSpace(PipelineId))
    {
      results.Add(new ValidationResult("PipelineId is required", [nameof(PipelineId)]));
    }

    if (string.IsNullOrWhiteSpace(Input))
    {
      results.Add(new ValidationResult("Input is required", [nameof(Input)]));
    }

    return results.Count is 0;
  }
}