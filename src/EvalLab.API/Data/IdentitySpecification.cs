using System.Linq.Expressions;

namespace EvalLab.API.Data;

sealed class IdentitySpecification<T> : FilterSpecification<T> where T : Entity
{
  public override Expression<Func<T, bool>> ToExpression()
  {
    return x => true;
  }
}