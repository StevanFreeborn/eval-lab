using System.Text.Json;
using System.Text.Json.Nodes;

using Json.Schema;

namespace EvalLab.API.Evaluations;

static class SuccessCriteriaType
{
  public const string Null = "null";
  public const string UnstructuredExactMatch = "Unstructured Exact Match";
  public const string UnstructuredPartialMatch = "Unstructured Partial Match";
  public const string JsonMatch = "JSON Match";
}

abstract class SuccessCriteria(string type)
{
  public string Type { get; init; } = type;
  public abstract bool IsSatisfiedBy(string output);
}

class NullSuccessCriteria : SuccessCriteria
{
  public NullSuccessCriteria() : base(SuccessCriteriaType.Null) { }
  public override bool IsSatisfiedBy(string output) => true;
}

class UnstructuredExactMatch : SuccessCriteria
{
  public string MatchValue { get; init; } = string.Empty;

  public UnstructuredExactMatch() : base(SuccessCriteriaType.UnstructuredExactMatch) { }

  public override bool IsSatisfiedBy(string output) => output == MatchValue;
}

class UnstructuredPartialMatch : SuccessCriteria
{
  public string MatchValue { get; init; } = string.Empty;

  public UnstructuredPartialMatch() : base(SuccessCriteriaType.UnstructuredPartialMatch) { }

  public override bool IsSatisfiedBy(string output) => output.Contains(MatchValue);
}

class JsonMatch : SuccessCriteria
{
  public string Schema { get; init; } = string.Empty;

  public JsonMatch() : base(SuccessCriteriaType.JsonMatch) { }

  public override bool IsSatisfiedBy(string output)
  {
    try
    {
      var schema = JsonSchema.FromText(Schema);
      var node = JsonNode.Parse(output);
      var results = schema.Evaluate(node);
      return results.IsValid;
    }
    catch (Exception ex) when (ex is JsonException)
    {
      return false;
    }
  }
}