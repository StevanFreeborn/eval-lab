namespace EvalLab.API.Data;

interface IRepository<T> where T : Entity
{
  Task<Page<T>> GetAsync(int pageNumber, int pageSize, FilterSpecification<T> spec, SortSpecification<T> sort);
  Task<T> GetAsync(FilterSpecification<T> spec);
  Task<T> CreateAsync(T entity);
  Task DeleteAsync(FilterSpecification<T> spec);
}