namespace TaskManager.Services
{
    using Microsoft.Extensions.Options;
    using TaskManager.Options;

    public class BackgroundTaskProcessor(
        TaskQueueService queueService,
        ILogger<BackgroundTaskProcessor> logger,
        IOptions<BackgroundTaskProcessorOptions> options,
        PauseService pauseService)
        : BackgroundService
    {
        private readonly int _workerCount = options.Value.WorkerCount;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < _workerCount; i++)
            {
                var workerId = i + 1;
                tasks.Add(Task.Run(async () =>
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        pauseService.WaitIfPaused(stoppingToken);
                        var task = await queueService.DequeueAsync(stoppingToken);
                        logger.LogInformation(" {timespan} - Worker {workerId}: {task}",
                            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                            workerId,
                            task);
                        await Task.Delay(1000, stoppingToken);
                    }
                }, stoppingToken));
            }
            return Task.WhenAll(tasks);
        }
    }
}