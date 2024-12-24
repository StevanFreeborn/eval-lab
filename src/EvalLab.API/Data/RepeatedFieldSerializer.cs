using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Google.Protobuf.Collections;

namespace EvalLab.API.Data;

class RepeatedFieldSerializer<T> : SerializerBase<RepeatedField<T>>
{
  private readonly IBsonSerializer<T> _itemSerializer;

  public RepeatedFieldSerializer()
  {
    _itemSerializer = BsonSerializer.LookupSerializer<T>();
  }

  public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, RepeatedField<T> value)
  {
    context.Writer.WriteStartArray();
    foreach (var item in value)
    {
      _itemSerializer.Serialize(context, item);
    }
    context.Writer.WriteEndArray();
  }

  public override RepeatedField<T> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
  {
    var field = new RepeatedField<T>();
    context.Reader.ReadStartArray();

    while (context.Reader.ReadBsonType() != MongoDB.Bson.BsonType.EndOfDocument)
    {
      field.Add(_itemSerializer.Deserialize(context));
    }

    context.Reader.ReadEndArray();
    return field;
  }
}
