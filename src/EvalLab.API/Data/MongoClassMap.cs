using EvalLab.API.Evaluations;
using EvalLab.API.Pipelines;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace EvalLab.API.Data;

static class MongoClassMap
{
  public static void Register()
  {
    BsonClassMap.RegisterClassMap<Entity>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
      cm.MapIdProperty(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
      cm.MapProperty(e => e.CreatedDate).SetElementName("createdDate");
      cm.MapProperty(e => e.UpdatedDate).SetElementName("updatedDate");
    });

    BsonClassMap.RegisterClassMap<Evaluation>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
      cm.MapProperty(e => e.Name).SetElementName("name");
      cm.MapProperty(e => e.Description).SetElementName("description");
    });

    BsonClassMap.RegisterClassMap<Pipeline>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
      cm.MapProperty(p => p.Name).SetElementName("name");
      cm.MapProperty(p => p.Description).SetElementName("description");
      cm.MapProperty(p => p.Endpoint).SetElementName("endpoint");
    });
  }
}