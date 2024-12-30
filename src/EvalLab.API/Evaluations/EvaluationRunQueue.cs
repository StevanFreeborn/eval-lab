using System.Threading.Channels;

namespace EvalLab.API.Evaluations;

interface IEvaluationRunQueue
{
  Task EnqueueAsync(EvaluationRun run);
  Task<EvaluationRun> DequeueAsync();
}

class EvaluationRunQueue : IEvaluationRunQueue
{
  private readonly Channel<EvaluationRun> _channel = Channel.CreateUnbounded<EvaluationRun>();

  public async Task EnqueueAsync(EvaluationRun run)
  {
    await _channel.Writer.WriteAsync(run);
  }

  public async Task<EvaluationRun> DequeueAsync()
  {
    return await _channel.Reader.ReadAsync();
  }
}