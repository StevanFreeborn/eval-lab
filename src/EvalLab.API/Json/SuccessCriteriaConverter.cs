using System.Text.Json;
using System.Text.Json.Serialization;

using EvalLab.API.Evaluations;

namespace EvalLab.API.Json;

class SuccessCriteriaConverter : JsonConverter<SuccessCriteria>
{
  public override SuccessCriteria Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var jsonDocument = JsonDocument.ParseValue(ref reader);
    var root = jsonDocument.RootElement;

    var type = root.GetProperty("type").GetString();

    return type switch
    {
      SuccessCriteriaType.Null => JsonSerializer.Deserialize<NullSuccessCriteria>(root.GetRawText(), options)!,
      SuccessCriteriaType.UnstructuredExactMatch => JsonSerializer.Deserialize<UnstructuredExactMatch>(root.GetRawText(), options)!,
      _ => throw new JsonException($"Unknown success criteria type: {type}")
    };
  }

  public override void Write(Utf8JsonWriter writer, SuccessCriteria value, JsonSerializerOptions options)
  {
    JsonSerializer.Serialize(writer, value, value.GetType(), options);
  }
}