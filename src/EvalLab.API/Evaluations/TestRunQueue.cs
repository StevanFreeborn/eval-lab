using System.Threading.Channels;

namespace EvalLab.API.Evaluations;

interface ITestRunQueue
{
  Task EnqueueAsync(TestRunQueueItem item);
  Task<TestRunQueueItem> DequeueAsync();
}

class TestRunQueue : ITestRunQueue
{
  private readonly Channel<TestRunQueueItem> _channel = Channel.CreateUnbounded<TestRunQueueItem>();

  public async Task EnqueueAsync(TestRunQueueItem item)
  {
    await _channel.Writer.WriteAsync(item);
  }

  public async Task<TestRunQueueItem> DequeueAsync()
  {
    return await _channel.Reader.ReadAsync();
  }
}