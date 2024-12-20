using EvalLab.API.Common;
using EvalLab.API.Data;

using Microsoft.AspNetCore.Mvc;

namespace EvalLab.API.Pipelines;

static class PipelineEndpoints
{
  public static WebApplication MapPipelineEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/pipelines");

    group.MapPost(string.Empty, async ([FromBody] AddPipelineDto dto, [FromServices] IRepository<Pipeline> repo) =>
    {
      if (dto.TryValidate(out var results) is false)
      {
        return Results.ValidationProblem(results.ToErrors());
      }

      var pipeline = dto.ToPipeline();
      await repo.CreateAsync(pipeline);
      return Results.Created($"/pipelines/{pipeline.Id}", PipelineDto.From(pipeline));
    });

    group.MapPost("{id}/run", (string id) => Results.Ok());

    return app;
  }
}