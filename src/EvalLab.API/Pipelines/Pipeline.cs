using EvalLab.API.Data;

namespace EvalLab.API.Pipelines;

class Pipeline : Entity
{
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public string Endpoint { get; init; } = string.Empty;

  // TODO: Null bad...use Result<T> instead
  public async Task<Run?> RunAsync(HttpClient client, RunRequest request)
  {
    var response = await client.PostAsJsonAsync(Endpoint, request);

    if (response.IsSuccessStatusCode is false)
    {
      return null;
    }

    var output = await response.Content.ReadFromJsonAsync<RunResponse>();

    if (output is null)
    {
      return null;
    }

    return new Run(request.Id, request.Input, output.Output);
  }
}