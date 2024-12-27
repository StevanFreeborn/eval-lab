using System.Net;

using EvalLab.API.Common;
using EvalLab.API.Data;

using Microsoft.AspNetCore.Mvc;

namespace EvalLab.API.Pipelines;

static class RunEndpoints
{
  public static WebApplication MapRunEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/runs");

    group.MapPost(
      string.Empty,
      async (
        [FromBody] RunRequest request,
        [FromServices] IRepository<Pipeline> pipelineRepo,
        [FromServices] IRepository<Run> runRepo,
        [FromServices] HttpClient client
      ) =>
    {
      if (request.TryValidate(out var results) is false)
      {
        return Results.ValidationProblem(results.ToErrors());
      }

      var pipeline = await pipelineRepo.GetAsync(FilterSpecification<Pipeline>.From(p => p.Id == request.PipelineId));

      if (pipeline is null)
      {
        return Results.NotFound();
      }

      var runResult = await pipeline.RunAsync(client, request);

      if (runResult.Failed)
      {
        return Results.Problem(
          title: "Failed to run pipeline",
          detail: "An error occurred while running the pipeline",
          statusCode: (int)HttpStatusCode.InternalServerError
        );
      }

      await runRepo.CreateAsync(runResult.Value);

      return Results.Created($"/runs/{runResult.Value.Id}", RunDto.From(runResult.Value));
    });

    group.MapGet(
      string.Empty,
      async (
        [FromServices] IRepository<Run> repo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sortBy = "createdDate",
        [FromQuery] string sortOrder = "desc",
        [FromQuery] string? pipelineId = null,
        [FromQuery] string? name = null
      ) =>
    {

      var spec = FilterSpecification<Run>.All;

      if (pipelineId is not null)
      {
        spec = spec.And(FilterSpecification<Run>.From(p => p.PipelineId == pipelineId));
      }

      if (name is not null)
      {
        spec = spec.And(FilterSpecification<Run>.From(p => p.Input.Contains(name)));
      }

      var sort = SortSpecification<Run>.From(sortBy, sortOrder);

      var page = await repo.GetAsync(pageNumber, pageSize, spec, sort);
      return Results.Ok(PageDto<RunDto>.FromPage(page, RunDto.From));
    });

    group.MapGet("{id}", async (string id, [FromServices] IRepository<Run> repo) =>
    {
      var run = await repo.GetAsync(FilterSpecification<Run>.From(p => p.Id == id));
      return run is not null ? Results.Ok(RunDto.From(run)) : Results.NotFound();
    });

    group.MapDelete("{id}", async (string id, [FromServices] IRepository<Run> repo) =>
    {
      var isDeleted = await repo.DeleteAsync(FilterSpecification<Run>.From(p => p.Id == id));
      return isDeleted ? Results.NoContent() : Results.NotFound();
    });

    return app;
  }
}