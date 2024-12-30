using EvalLab.API.Traces;

using MongoDB.Driver;

namespace EvalLab.API.Data;

static class Extensions
{
  public static async Task AddIndexes(this IServiceCollection services)
  {
    var dbContext = services.BuildServiceProvider().GetRequiredService<MongoDbContext>();

    var runIdIndex = Builders<Trace>.IndexKeys.Ascending(t => t.PipelineRunId);

    var traceRunIdIndex = new CreateIndexModel<Trace>(
      runIdIndex,
      new() { Name = "run_id_index" }
    );

    await dbContext.GetCollection<Trace>().Indexes.CreateOneAsync(traceRunIdIndex);
  }
}