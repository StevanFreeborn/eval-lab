using System.Net;

using EvalLab.API.Common;
using EvalLab.API.Data;

using Microsoft.AspNetCore.Mvc;

namespace EvalLab.API.Pipelines;

static class PipelineRunEndpoints
{
  public static WebApplication MapPipelineRunEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/pipelines/runs");

    group.MapPost(
      string.Empty,
      async (
        [FromBody] PipelineRunRequest request,
        [FromServices] IRepository<Pipeline> pipelineRepo,
        [FromServices] IRepository<PipelineRun> runRepo,
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

      return Results.Created($"/runs/{runResult.Value.Id}", PipelineRunDto.From(runResult.Value));
    });

    group.MapGet(
      string.Empty,
      async (
        [FromServices] IRepository<PipelineRun> repo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sortBy = "createdDate",
        [FromQuery] string sortOrder = "desc",
        [FromQuery] string? pipelineId = null,
        [FromQuery] string? name = null
      ) =>
    {

      var spec = FilterSpecification<PipelineRun>.All;

      if (pipelineId is not null)
      {
        spec = spec.And(FilterSpecification<PipelineRun>.From(p => p.PipelineId == pipelineId));
      }

      if (name is not null)
      {
        spec = spec.And(FilterSpecification<PipelineRun>.From(p => p.Input.Contains(name)));
      }

      var sort = SortSpecification<PipelineRun>.From(sortBy, sortOrder);

      var page = await repo.GetAsync(pageNumber, pageSize, spec, sort);
      return Results.Ok(PageDto<PipelineRunDto>.FromPage(page, PipelineRunDto.From));
    });

    group.MapGet("{id}", async (string id, [FromServices] IRepository<PipelineRun> repo) =>
    {
      var run = await repo.GetAsync(FilterSpecification<PipelineRun>.From(p => p.Id == id));
      return run is not null ? Results.Ok(PipelineRunDto.From(run)) : Results.NotFound();
    });

    group.MapDelete("{id}", async (string id, [FromServices] IRepository<PipelineRun> repo) =>
    {
      var isDeleted = await repo.DeleteAsync(FilterSpecification<PipelineRun>.From(p => p.Id == id));
      return isDeleted ? Results.NoContent() : Results.NotFound();
    });

    return app;
  }
}