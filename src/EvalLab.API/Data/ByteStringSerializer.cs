using Google.Protobuf;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace EvalLab.API.Data;

class ByteStringSerializer : SerializerBase<ByteString>
{
  public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ByteString value)
  {
    if (value is not null)
    {
      var str = Convert.ToHexString(value.ToByteArray());
      context.Writer.WriteString(str);
      return;
    }

    context.Writer.WriteNull();
  }

  public override ByteString Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
  {
    var bsonType = context.Reader.GetCurrentBsonType();

    if (bsonType == MongoDB.Bson.BsonType.String)
    {
      var bytes = Convert.FromHexString(context.Reader.ReadString());
      return ByteString.CopyFrom(bytes);
    }

    if (bsonType == MongoDB.Bson.BsonType.Null)
    {
      context.Reader.ReadNull();
      return ByteString.Empty;
    }

    throw new InvalidOperationException($"Cannot deserialize ByteString from BsonType {bsonType}");
  }
}
