using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

using Google.Protobuf;

using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Trace.V1;

namespace EvalLab.API.Traces;

static class TracesDataExtensions
{
  private static readonly JsonSerializerOptions JsonSerializerOptions = new()
  {
    WriteIndented = false
  };

  public static Trace ToTrace(this TracesData data, string runId)
  {
    var spans = data.ResourceSpans
      .SelectMany(rs => rs.ScopeSpans)
      .SelectMany(ss => ss.Spans)
      .ToArray();

    return new() { PipelineRunId = runId, Spans = [.. spans.Select(s => s.ToTraceSpan())] };
  }

  private static TraceSpan ToTraceSpan(this Span span)
  {
    var attributes = span.Attributes.ToDictionary(a => a.Key, a => a.Value.GetString());

    return new()
    {
      ParentId = span.ParentSpanId.ToHexString(),
      Id = span.SpanId.ToHexString(),
      Name = span.Name,
      Start = span.StartTimeUnixNano / 1_000_000,
      End = span.EndTimeUnixNano / 1_000_000,
      Attributes = attributes
    };
  }

  private static string GetString(this AnyValue value) => value.ValueCase switch
  {
    AnyValue.ValueOneofCase.StringValue => value.StringValue,
    AnyValue.ValueOneofCase.IntValue => value.IntValue.ToString(CultureInfo.InvariantCulture),
    AnyValue.ValueOneofCase.DoubleValue => value.DoubleValue.ToString(CultureInfo.InvariantCulture),
    AnyValue.ValueOneofCase.BoolValue => value.BoolValue ? "true" : "false",
    AnyValue.ValueOneofCase.BytesValue => value.BytesValue.ToHexString(),
    AnyValue.ValueOneofCase.ArrayValue => ConvertAnyValue(value)!.ToJsonString(JsonSerializerOptions),
    AnyValue.ValueOneofCase.KvlistValue => ConvertAnyValue(value)!.ToJsonString(JsonSerializerOptions),
    AnyValue.ValueOneofCase.None => string.Empty,
    _ => value.ToString(),
  };

  private static string ToHexString(this ByteString byteString) => Convert.ToHexString(byteString.ToByteArray());

  private static JsonNode? ConvertAnyValue(AnyValue value)
  {
    return value.ValueCase switch
    {
      AnyValue.ValueOneofCase.StringValue => JsonValue.Create(value.StringValue),
      AnyValue.ValueOneofCase.IntValue => JsonValue.Create(value.IntValue),
      AnyValue.ValueOneofCase.DoubleValue => JsonValue.Create(value.DoubleValue),
      AnyValue.ValueOneofCase.BoolValue => JsonValue.Create(value.BoolValue),
      AnyValue.ValueOneofCase.BytesValue => JsonValue.Create(value.BytesValue.ToHexString()),
      AnyValue.ValueOneofCase.ArrayValue => ConvertArray(value.ArrayValue),
      AnyValue.ValueOneofCase.KvlistValue => ConvertKeyValues(value.KvlistValue),
      AnyValue.ValueOneofCase.None => null,
      _ => throw new InvalidOperationException($"Unexpected AnyValue type: {value.ValueCase}"),
    };

    static JsonArray ConvertArray(ArrayValue value)
    {
      var a = new JsonArray();

      foreach (var item in value.Values)
      {
        a.Add(ConvertAnyValue(item));
      }

      return a;
    }

    static JsonObject ConvertKeyValues(KeyValueList value)
    {
      var o = new JsonObject();

      foreach (var item in value.Values)
      {
        o[item.Key] = ConvertAnyValue(item.Value);
      }

      return o;
    }
  }
}