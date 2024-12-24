using EvalLab.API.Traces;

namespace EvalLab.API.Data;

class MongoTraceRepository(MongoDbContext context) : MongoRepository<Trace>(context)
{
}