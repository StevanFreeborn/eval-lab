using System.ComponentModel.DataAnnotations;

namespace EvalLab.API.Pipelines;

record AddPipelineDto(string Name, string Description, string Endpoint)
{
  public bool TryValidate(out List<ValidationResult> results)
  {
    results = [];

    if (string.IsNullOrWhiteSpace(Name))
    {
      results.Add(new ValidationResult("Name is required", [nameof(Name)]));
    }

    if (string.IsNullOrWhiteSpace(Endpoint))
    {
      results.Add(new ValidationResult("Endpoint is required", [nameof(Endpoint)]));
    }

    return results.Count is 0;
  }

  public Pipeline ToPipeline() => new()
  {
    Name = Name,
    Description = Description,
    Endpoint = Endpoint,
  };
}