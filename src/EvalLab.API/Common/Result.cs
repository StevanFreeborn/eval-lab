namespace EvalLab.API.Common;

public class Result
{
  public bool Succeeded { get; private set; } = false;
  public bool Failed => !Succeeded;
  public Exception Error { get; private set; } = null!;

  protected Result(bool succeeded, Exception error)
  {
    if (succeeded && error is not null)
    {
      throw new InvalidOperationException("A successful result cannot have an error.");
    }

    Succeeded = succeeded;
    Error = error!;
  }

  public static Result Success() => new(true, null!);

  public static Result Failure(Exception error)
  {
    if (error is null)
    {
      throw new ArgumentNullException(nameof(error), "Error cannot be null for a failed result.");
    }

    return new Result(false, error);
  }
}

public class Result<T> : Result
{
  public T Value { get; private set; } = default!;

  private Result(bool succeeded, T value, Exception error) : base(succeeded, error)
  {
    if (succeeded && error is not null)
    {
      throw new InvalidOperationException("A successful result cannot have an error.");
    }

    if (succeeded is false && value is not null)
    {
      throw new InvalidOperationException("A failed result cannot have a value.");
    }

    Value = value;
  }

  public static Result<T> Success(T value) => new Result<T>(true, value, null!);

  public static new Result<T> Failure(Exception error)
  {
    if (error is null)
    {
      throw new ArgumentNullException(nameof(error), "Error cannot be null for a failed result.");
    }

    return new Result<T>(false, default!, error);
  }
}
