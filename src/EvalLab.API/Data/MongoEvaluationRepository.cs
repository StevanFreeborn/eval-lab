using EvalLab.API.Evaluations;

namespace EvalLab.API.Data;

class MongoEvaluationRepository(MongoDbContext context) : MongoRepository<Evaluation>(context)
{
}