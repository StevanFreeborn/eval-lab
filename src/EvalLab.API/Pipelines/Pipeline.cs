using EvalLab.API.Common;
using EvalLab.API.Data;

namespace EvalLab.API.Pipelines;

class Pipeline : Entity
{
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public string Endpoint { get; init; } = string.Empty;
  public List<Run> Runs { get; init; } = [];

  public async Task<Result<Run>> RunAsync(HttpClient client, RunRequest request)
  {
    try
    {
      var response = await client.PostAsJsonAsync(Endpoint, request);
      var output = await response.Content.ReadFromJsonAsync<RunResponse>();
      return Result<Run>.Success(new Run()
      {
        PipelineId = request.PipelineId,
        Id = request.Id,
        Input = request.Input,
        Output = output?.Output ?? string.Empty
      });
    }
    catch (Exception ex)
    {
      return Result<Run>.Failure(ex);
    }
  }
}