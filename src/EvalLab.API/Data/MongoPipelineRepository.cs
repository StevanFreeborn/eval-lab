using EvalLab.API.Pipelines;

namespace EvalLab.API.Data;

class MongoPipelineRepository(MongoDbContext context) : MongoRepository<Pipeline>(context)
{
}