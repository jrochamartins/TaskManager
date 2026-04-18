namespace TaskManager.Services;

public class PauseServiceTcs : IPauseService
{
    private volatile TaskCompletionSource<bool> _tcs = new();

    public PauseServiceTcs(bool initialState = false)
    {
        if (initialState)
            _tcs.SetResult(true);
    }

    public Task WaitIfPaused(CancellationToken cancellationToken) =>
        _tcs.Task;

    public void Pause()
    {
        var tcs = _tcs;
        if (tcs.Task.IsCompleted)
            Interlocked.CompareExchange(ref _tcs, new TaskCompletionSource<bool>(), tcs);
    }

    public void Resume() =>
        _tcs.TrySetResult(true);
}
