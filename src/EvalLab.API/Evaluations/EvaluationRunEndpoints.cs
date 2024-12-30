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
        [FromServices] ITestRunQueue testRunQueue
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

      foreach (var num in Enumerable.Range(0, evaluationRun.SampleSize))
      {
        await testRunQueue.EnqueueAsync(new(evaluationRun.Id, num));
      }

      return Results.Created($"/evaluations/runs/{evaluationRun.Id}", EvaluationRunDto.From(evaluationRun));
    });

    group.MapGet(
      string.Empty,
      async (
        [FromServices] IRepository<EvaluationRun> repo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50
      ) =>
    {
      var page = await repo.GetAsync(
        pageNumber,
        pageSize,
        FilterSpecification<EvaluationRun>.All,
        SortSpecification<EvaluationRun>.SortByDesc(er => er.CreatedDate)
      );

      return Results.Ok(PageDto<EvaluationRunDto>.FromPage(page, EvaluationRunDto.From));
    });

    group.MapGet("{id}", async (string id, [FromServices] IRepository<EvaluationRun> repo) =>
    {
      var evaluationRun = await repo.GetAsync(FilterSpecification<EvaluationRun>.From(er => er.Id == id));
      return evaluationRun is not null ? Results.Ok(EvaluationRunDto.From(evaluationRun)) : Results.NotFound();
    });

    return app;
  }
}