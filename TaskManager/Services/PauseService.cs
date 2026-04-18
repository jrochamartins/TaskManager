namespace TaskManager.Services
{
    public class PauseService
    {
        private readonly ManualResetEventSlim _pauseEvent = new(false); // Começa pausado
        public bool IsPaused { get; private set; } = true;

        public void Pause()
        {
            IsPaused = true;
            _pauseEvent.Reset();
        }

        public void Resume()
        {
            IsPaused = false;
            _pauseEvent.Set();
        }

        public void WaitIfPaused(CancellationToken cancellationToken)
        {
            _pauseEvent.Wait(cancellationToken);
        }
    }
}
