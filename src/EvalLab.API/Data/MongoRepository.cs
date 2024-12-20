using System.Linq.Expressions;

using MongoDB.Driver;

namespace EvalLab.API.Data;

abstract class MongoRepository<T>(MongoDbContext context) : IRepository<T> where T : Entity
{
  private const string CountFacetName = "count";
  private const string ItemsFacetName = "items";
  private protected readonly IMongoCollection<T> _collection = context.GetCollection<T>();

  public virtual async Task<Page<T>> GetAsync(int pageNumber, int pageSize)
  {
    var totalFacet = AggregateFacet.Create(
      CountFacetName,
      PipelineDefinition<T, AggregateCountResult>.Create([PipelineStageDefinitionBuilder.Count<T>()])
    );


    var itemsFacet = AggregateFacet.Create(
      ItemsFacetName,
      PipelineDefinition<T, T>.Create(
        [
          PipelineStageDefinitionBuilder.Skip<T>((pageNumber - 1) * pageSize),
          PipelineStageDefinitionBuilder.Limit<T>(pageSize)
        ]
      )
    );

    var aggregation = await _collection.Aggregate()
      .SortByDescending(e => e.CreatedDate)
      .Facet(totalFacet, itemsFacet)
      .FirstOrDefaultAsync();

    var countOutput = aggregation.Facets.First(f => f.Name == CountFacetName).Output<AggregateCountResult>();
    var totalItems = countOutput.Count is 0 ? 0 : countOutput[0].Count;
    var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
    var items = aggregation.Facets.First(f => f.Name == ItemsFacetName).Output<T>();

    return new(pageNumber, pageSize, totalPages, totalItems, items);
  }

  public virtual async Task<T> GetAsync(Expression<Func<T, bool>> filter)
  {
    return await _collection.Find(filter).FirstOrDefaultAsync();
  }

  public virtual async Task<T> CreateAsync(T entity)
  {
    await _collection.InsertOneAsync(entity);
    return entity;
  }

  public virtual async Task DeleteAsync(Expression<Func<T, bool>> filter)
  {
    await _collection.DeleteOneAsync(filter);
  }
}