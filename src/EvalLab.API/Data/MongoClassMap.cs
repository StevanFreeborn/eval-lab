using EvalLab.API.Evaluations;
using EvalLab.API.Pipelines;
using EvalLab.API.Traces;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;

namespace EvalLab.API.Data;

static class MongoClassMap
{
  public static void Register()
  {
    ConventionRegistry.Register("CamelCase", new ConventionPack { new CamelCaseElementNameConvention() }, _ => true);

    BsonClassMap.TryRegisterClassMap<Entity>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
      cm.MapIdProperty(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
    });

    BsonClassMap.TryRegisterClassMap<Evaluation>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.TryRegisterClassMap<EvaluationRun>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.TryRegisterClassMap<TestRun>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.TryRegisterClassMap<SuccessCriteria>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
      cm.SetIsRootClass(true);
    });

    BsonClassMap.TryRegisterClassMap<NullSuccessCriteria>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.TryRegisterClassMap<UnstructuredExactMatch>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.TryRegisterClassMap<Pipeline>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.TryRegisterClassMap<PipelineRun>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.TryRegisterClassMap<Trace>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.TryRegisterClassMap<TraceSpan>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });
  }
}