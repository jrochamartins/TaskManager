namespace TaskManager.Services;

public class PauseService
{
    private readonly ManualResetEventSlim _pauseEvent = new(false); // Começa pausado

    public void Pause()
    {
        _pauseEvent.Reset();
    }

    public void Resume()
    {
        _pauseEvent.Set();
    }

    public void WaitIfPaused(CancellationToken cancellationToken)
    {
        _pauseEvent.Wait(cancellationToken);
    }
}