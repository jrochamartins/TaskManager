using System.Threading.Channels;

namespace TaskManager.Services;

public class TaskQueueService
{
    private readonly Channel<string> _channel = Channel.CreateBounded<string>(
        new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });

    public async ValueTask EnqueueAsync(string task)
    {
        await _channel.Writer.WriteAsync(task);
    }

    public async ValueTask<string> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }
}