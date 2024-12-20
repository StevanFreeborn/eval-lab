namespace EvalLab.API.Data;

abstract class Entity
{
  public string Id { get; init; } = null!;
  public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedDate { get; init; } = DateTime.UtcNow;
}