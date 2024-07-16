using Quartz;
using Quartz.Spi;

namespace BeerEconomy.PriceCollectorService.Schedule;

internal class QuartzHostedService(
        ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        IEnumerable<JobSchedule> jobSchedules)
    : IHostedService
{
    private IScheduler _scheduler = null!;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        _scheduler.JobFactory = jobFactory;

        foreach (var jobSchedule in jobSchedules)
        {
            var job = CreateJob(jobSchedule);
            var trigger = CreateTrigger(jobSchedule);

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        await _scheduler.Start(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _scheduler.Shutdown(cancellationToken);
    }

    private static IJobDetail CreateJob(JobSchedule schedule)
    {
        var jobType = schedule.JobType;
        return JobBuilder
            .Create(jobType)
            .WithIdentity(jobType.FullName!)
            .WithDescription(jobType.Name)
            .Build();
    }

    private static ITrigger CreateTrigger(JobSchedule schedule)
    {
        return TriggerBuilder
            .Create()
            .WithIdentity($"{schedule.JobType.FullName}.trigger")
            .WithCronSchedule(schedule.CronExpression)
            .WithDescription(schedule.CronExpression)
            .Build();
    }
}