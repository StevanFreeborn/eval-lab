using EvalLab.API.Evaluations;
using EvalLab.API.Pipelines;
using EvalLab.API.Traces;

using Google.Protobuf.Collections;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;

using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Trace.V1;

namespace EvalLab.API.Data;

static class MongoClassMap
{
  public static void Register()
  {
    ConventionRegistry.Register("CamelCase", new ConventionPack { new CamelCaseElementNameConvention() }, _ => true);

    BsonClassMap.RegisterClassMap<Entity>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
      cm.MapIdProperty(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
    });

    BsonClassMap.RegisterClassMap<Evaluation>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.RegisterClassMap<Pipeline>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.RegisterClassMap<Trace>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
    });

    BsonClassMap.RegisterClassMap<Span>(cm =>
    {
      var byteStringSerializer = new ByteStringSerializer();

      cm.AutoMap();
      cm.MapMember(s => s.TraceId).SetSerializer(byteStringSerializer);
      cm.MapMember(s => s.SpanId).SetSerializer(byteStringSerializer);
      cm.MapMember(s => s.ParentSpanId).SetSerializer(byteStringSerializer);
      cm.MapMember(s => s.Attributes).SetSerializer(new RepeatedFieldSerializer<KeyValue>());
    });
  }
}