using EvalLab.API.Evaluations;
using EvalLab.API.Traces;

using MongoDB.Driver;

namespace EvalLab.API.Data;

static class Extensions
{
  public static async Task AddIndexes(this IServiceCollection services)
  {
    var dbContext = services.BuildServiceProvider().GetRequiredService<MongoDbContext>();

    var traceByPipelineRunIdIndex = Builders<Trace>.IndexKeys.Ascending(t => t.PipelineRunId);
    var traceByPipelineRunIdIndexModel = new CreateIndexModel<Trace>(
      traceByPipelineRunIdIndex,
      new() { Name = "pipeline_run_id_index" }
    );

    await dbContext.GetCollection<Trace>().Indexes.CreateOneAsync(traceByPipelineRunIdIndexModel);

    var evaluationRunByEvaluationIdIndex = Builders<EvaluationRun>.IndexKeys.Ascending(er => er.EvaluationId);
    var evaluationRunByEvaluationIdIndexModel = new CreateIndexModel<EvaluationRun>(
      evaluationRunByEvaluationIdIndex,
      new() { Name = "evaluation_id_index" }
    );

    await dbContext.GetCollection<EvaluationRun>().Indexes.CreateOneAsync(evaluationRunByEvaluationIdIndexModel);
  }
}