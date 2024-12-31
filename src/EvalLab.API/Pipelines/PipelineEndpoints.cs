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

    group.MapGet(
      string.Empty,
      async (
        [FromServices] IRepository<Pipeline> repo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sortBy = "createdDate",
        [FromQuery] string sortOrder = "desc",
        [FromQuery] string? name = null
      ) =>
    {
      var spec = FilterSpecification<Pipeline>.All;

      if (name is not null)
      {
        spec = spec.And(FilterSpecification<Pipeline>.From(p => p.Name.Contains(name)));
      }

      var sort = SortSpecification<Pipeline>.From(sortBy, sortOrder);

      var page = await repo.GetAsync(pageNumber, pageSize, spec, sort);
      return Results.Ok(PageDto<PipelineDto>.FromPage(page, PipelineDto.From));
    });

    group.MapGet("{id}", async (string id, [FromServices] IRepository<Pipeline> repo) =>
    {
      var pipeline = await repo.GetAsync(FilterSpecification<Pipeline>.From(p => p.Id == id));
      return pipeline is not null ? Results.Ok(PipelineDto.From(pipeline)) : Results.NotFound();
    });

    group.MapDelete("{id}", async (string id, [FromServices] IRepository<Pipeline> pipelineRepo, [FromServices] IRepository<PipelineRun> pipelineRunRepo) =>
    {
      var isDeleted = await pipelineRepo.DeleteAsync(FilterSpecification<Pipeline>.From(p => p.Id == id));

      if (isDeleted)
      {
        await pipelineRunRepo.DeleteManyAsync(FilterSpecification<PipelineRun>.From(pr => pr.PipelineId == id));
      }

      return isDeleted ? Results.NoContent() : Results.NotFound();
    });

    return app;
  }
}