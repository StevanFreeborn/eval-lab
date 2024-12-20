using System.Linq.Expressions;

namespace EvalLab.API.Data;

interface IRepository<T> where T : Entity
{
  Task<Page<T>> GetAsync(int pageNumber, int pageSize);
  Task<T> GetAsync(Expression<Func<T, bool>> filter);
  Task<T> CreateAsync(T entity);
  Task DeleteAsync(Expression<Func<T, bool>> filter);
}