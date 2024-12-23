using EvalLab.API.Common;
using EvalLab.API.Data;

using Microsoft.AspNetCore.Mvc;

namespace EvalLab.API.Evaluations;

static class EvaluationEndpoints
{
  public static WebApplication MapEvaluationEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/evaluations");

    group.MapPost(string.Empty, async ([FromBody] AddEvaluationDto dto, [FromServices] IRepository<Evaluation> repo) =>
    {
      if (dto.TryValidate(out var results) is false)
      {
        return Results.ValidationProblem(results.ToErrors());
      }

      var evaluation = dto.ToEvaluation();
      await repo.CreateAsync(evaluation);
      return Results.Created($"/evaluations/{evaluation.Id}", EvaluationDto.From(evaluation));
    });

    group.MapGet(string.Empty, async ([FromServices] IRepository<Evaluation> repo, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50) =>
    {
      var page = await repo.GetAsync(
        pageNumber,
        pageSize,
        FilterSpecification<Evaluation>.All,
        SortSpecification<Evaluation>.SortByDesc(e => e.CreatedDate)
      );

      return Results.Ok(PageDto<EvaluationDto>.FromPage(page, EvaluationDto.From));
    });

    group.MapGet("{id}", async (string id, [FromServices] IRepository<Evaluation> repo) =>
    {
      var evaluation = await repo.GetAsync(FilterSpecification<Evaluation>.From(e => e.Id == id));
      return evaluation is not null ? Results.Ok(EvaluationDto.From(evaluation)) : Results.NotFound();
    });

    group.MapDelete("{id}", async (string id, [FromServices] IRepository<Evaluation> repo) =>
    {
      await repo.DeleteAsync(FilterSpecification<Evaluation>.From(e => e.Id == id));
      return Results.NoContent();
    });

    return app;
  }
}