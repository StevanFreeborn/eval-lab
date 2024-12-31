using System.Linq.Expressions;

namespace EvalLab.API.Data;

abstract class Entity
{
  public string Id { get; init; } = null!;
  public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedDate { get; init; } = DateTime.UtcNow;
  public static Dictionary<string, Expression<Func<T, object>>> SortMap<T>()
  {
    var map = typeof(T).GetProperties()
      .Where(prop => prop.CanRead)
      .ToDictionary(
        prop => prop.Name.ToLowerInvariant(),
        prop =>
        {
          var parameter = Expression.Parameter(typeof(T), "x");
          var propertyAccess = Expression.Property(parameter, prop);
          var convertToObject = Expression.Convert(propertyAccess, typeof(object));

          return Expression.Lambda<Func<T, object>>(convertToObject, parameter);
        }
      );

    return map;
  }
}