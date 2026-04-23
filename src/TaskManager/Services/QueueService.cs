using Microsoft.Extensions.Options;
using System.Threading.Channels;
using TaskManager.Options;

namespace TaskManager.Services;

public class QueueService(
    IOptions<QueueServiceOptions> options)
{
    private readonly Channel<string> _channel = Channel.CreateBounded<string>(
        new BoundedChannelOptions(options.Value.Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });

    public async ValueTask EnqueueAsync(string task) =>
        await _channel.Writer.WriteAsync(task);

    public async ValueTask<string> DequeueAsync(CancellationToken ct) =>
        await _channel.Reader.ReadAsync(ct);
}