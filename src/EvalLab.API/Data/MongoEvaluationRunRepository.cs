using EvalLab.API.Evaluations;

namespace EvalLab.API.Data;

class MongoEvaluationRunRepository(MongoDbContext context) : MongoRepository<EvaluationRun>(context)
{
}