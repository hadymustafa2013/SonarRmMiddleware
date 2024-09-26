using Quartz;

namespace RmMiddleware.Jobs;

public class TestJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        return Task.CompletedTask;
    }
}