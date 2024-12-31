namespace EvalLab.API.Evaluations;

static class SuccessCriteriaType
{
  public const string Null = "null";
  public const string UnstructuredExactMatch = "Unstructured Exact Match";
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