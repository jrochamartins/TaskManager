namespace TaskManager.Services;

public class PauseService
{
    private TaskCompletionSource<bool> _tcs = new();

    public PauseService(bool initialState = true)
    {
        if (initialState)
            _tcs.SetResult(true);
    }

    public Task WaitIfPaused() =>
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
