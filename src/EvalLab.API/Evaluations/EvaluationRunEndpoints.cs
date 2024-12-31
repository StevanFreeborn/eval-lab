using EvalLab.API.Common;
using EvalLab.API.Data;

using Microsoft.AspNetCore.Mvc;

namespace EvalLab.API.Evaluations;

static class EvaluationRunEndpoints
{
  public static WebApplication MapEvaluationRunEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/evaluations/runs");

    group.MapPost(
      string.Empty,
      async (
        [FromBody] AddEvaluationRunDto dto,
        [FromServices] IRepository<Evaluation> evaluationRepo,
        [FromServices] IRepository<EvaluationRun> evaluationRunRepo,
        [FromServices] IEvaluationRunQueue evaluationRunQueue
      ) =>
    {
      if (dto.TryValidate(out var results) is false)
      {
        return Results.ValidationProblem(results.ToErrors());
      }

      var evaluationRun = dto.ToEvaluationRun();
      var evaluation = dto.Evaluation.ToEvaluation();

      var numOfExistingRuns = await evaluationRunRepo.CountAsync(
        FilterSpecification<EvaluationRun>.From(er => er.EvaluationId == evaluationRun.EvaluationId)
      );

      // We only allow modifying the evaluation if there are no existing runs.
      // In order for runs to be usefully comparable, they must be based on the same
      // - pipeline
      // - input
      // - success criteria
      if (numOfExistingRuns is 0)
      {
        await evaluationRepo.UpdateAsync(
          FilterSpecification<Evaluation>.From(e => e.Id == evaluationRun.EvaluationId),
          evaluation
        );
      }

      await evaluationRunRepo.CreateAsync(evaluationRun);

      await evaluationRunQueue.EnqueueAsync(evaluationRun);

      return Results.Created($"/evaluations/runs/{evaluationRun.Id}", EvaluationRunDto.From(evaluationRun));
    });

    group.MapGet(
      string.Empty,
      async (
        [FromServices] IRepository<EvaluationRun> repo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? evaluationId = null
      ) =>
    {
      var filterSpec = FilterSpecification<EvaluationRun>.All;

      if (evaluationId is not null)
      {
        filterSpec = filterSpec.And(FilterSpecification<EvaluationRun>.From(er => er.EvaluationId == evaluationId));
      }

      var page = await repo.GetAsync(
        pageNumber,
        pageSize,
        filterSpec,
        SortSpecification<EvaluationRun>.SortByDesc(er => er.CreatedDate)
      );

      return Results.Ok(PageDto<EvaluationRunDto>.FromPage(page, EvaluationRunDto.From));
    });


    // TODO: include test runs for a given evaluation run
    group.MapGet("{id}", async (string id, [FromServices] IRepository<EvaluationRun> repo) =>
    {
      var evaluationRun = await repo.GetAsync(FilterSpecification<EvaluationRun>.From(er => er.Id == id));
      return evaluationRun is not null ? Results.Ok(EvaluationRunDto.From(evaluationRun)) : Results.NotFound();
    });

    group.MapDelete("{id}", async (string id, [FromServices] IRepository<EvaluationRun> repo) =>
    {
      var isDeleted = await repo.DeleteAsync(FilterSpecification<EvaluationRun>.From(er => er.Id == id));
      return isDeleted ? Results.NoContent() : Results.NotFound();
    });

    return app;
  }
}