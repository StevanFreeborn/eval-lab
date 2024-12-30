using EvalLab.API.Evaluations;
using EvalLab.API.Pipelines;

namespace EvalLab.API.Data;

class MongoEvaluationRunRepository(MongoDbContext context) : MongoRepository<EvaluationRun>(context)
{
}