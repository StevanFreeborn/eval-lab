using System.Diagnostics;
using System.Text;

using AnthropicClient;
using AnthropicClient.Models;

using EvalLab.API.Common;
using EvalLab.API.Pipelines;

using Microsoft.AspNetCore.Mvc;

namespace EvalLab.API.Demo;

static class DemoEndpoints
{
  public static WebApplication MapDemoEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/demo");

    group.MapPost("anthropic", async ([FromBody] PipelineRunRequest dto, [FromServices] IAnthropicApiClient client) =>
    {
      using var activity = Activity.Current?.Source.StartActivity("RunAnthropic");
      activity?.SetTag("evallab.run", $"run-{dto.Id}");
      activity?.SetTag("input", dto.Input);

      if (dto.TryValidate(out var results) is false)
      {
        return Results.ValidationProblem(results.ToErrors());
      }

      var response = await client.CreateMessageAsync(new MessageRequest(
        AnthropicModels.Claude3Haiku,
        [
          new(
            MessageRole.User,
            [new TextContent(dto.Input)]
          )
        ]
      ));

      var output = response.IsFailure
        ? "Failed to generate response"
        : response.Value.Content.OfType<TextContent>().Aggregate(new StringBuilder(), (acc, content) => acc.Append(content.Text)).ToString();

      activity?.SetTag("output", output);

      return Results.Ok(new PipelineRunResponse(output));
    });

    return app;
  }
}