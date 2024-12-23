using System.Linq.Expressions;

namespace EvalLab.API.Data;

abstract class SortSpecification<T> where T : Entity
{
  public const string Ascending = "asc";
  public const string Descending = "desc";
  public abstract Expression<Func<T, object>> ToExpression();

  public static SortSpecification<T> SortBy(Expression<Func<T, object>> expression) => new SortBySpecification<T>(expression);
  public static SortSpecification<T> SortByDesc(Expression<Func<T, object>> expression) => new SortByDescSpecification<T>(expression);

  public static SortSpecification<T> From(string sortBy, string sortOrder)
  {
    var sortMap = Entity.SortMap<T>();

    var key = sortBy.ToLowerInvariant();

    if (sortMap.TryGetValue(key, out var value) is false || value is null)
    {
      throw new ArgumentException($"{nameof(sortBy)} is invalid: {sortBy}");
    }

    return sortOrder.ToLowerInvariant() switch
    {
      Ascending => new SortBySpecification<T>(value),
      Descending => new SortByDescSpecification<T>(value),
      _ => throw new ArgumentException($"Invalid sort order: {sortOrder}")
    };
  }
}