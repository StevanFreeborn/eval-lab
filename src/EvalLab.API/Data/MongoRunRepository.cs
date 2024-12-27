using EvalLab.API.Pipelines;

namespace EvalLab.API.Data;

class MongoRunRepository(MongoDbContext context) : MongoRepository<Run>(context)
{
}