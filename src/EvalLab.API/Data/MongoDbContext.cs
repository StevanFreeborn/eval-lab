using EvalLab.API.Evaluations;
using EvalLab.API.Pipelines;
using EvalLab.API.Traces;

using MongoDB.Driver;

namespace EvalLab.API.Data;

class MongoDbContext
{
  private const string DatabaseName = "evallab";
  private const string EvaluationCollectionName = "evaluations";
  private const string EvaluationRunCollectionName = "evaluationRuns";
  private const string PipelineCollectionName = "pipelines";
  private const string PipelineRunCollectionName = "pipelineRuns";
  private const string TracesCollectionName = "traces";

  private readonly IMongoDatabase _database;

  public MongoDbContext(IMongoClient client)
  {
    MongoClassMap.Register();
    _database = client.GetDatabase(DatabaseName);
  }

  public IMongoCollection<T> GetCollection<T>() where T : Entity
  {
    return typeof(T) switch
    {
      Type t when t == typeof(Evaluation) => _database.GetCollection<T>(EvaluationCollectionName),
      Type t when t == typeof(EvaluationRun) => _database.GetCollection<T>(EvaluationRunCollectionName),
      Type t when t == typeof(Pipeline) => _database.GetCollection<T>(PipelineCollectionName),
      Type t when t == typeof(PipelineRun) => _database.GetCollection<T>(PipelineRunCollectionName),
      Type t when t == typeof(Trace) => _database.GetCollection<T>(TracesCollectionName),
      _ => throw new ArgumentException($"Collection for type {typeof(T).Name} not found.")
    };
  }
}