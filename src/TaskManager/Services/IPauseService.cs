namespace TaskManager.Services;

public interface IPauseService
{
    void Pause();
    void Resume();
    Task WaitIfPaused(CancellationToken cancellationToken);
}
