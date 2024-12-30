using EvalLab.API.Pipelines;

namespace EvalLab.API.Data;

class MongoPipelineRunRepository(MongoDbContext context) : MongoRepository<PipelineRun>(context)
{
}