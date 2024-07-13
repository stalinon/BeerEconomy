using Quartz;
using Quartz.Spi;

namespace BeerEconomy.PriceCollectorService.Schedule;

internal class SingletonJobFactory(IServiceProvider serviceProvider) : IJobFactory
{
    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) => (serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob)!;

    public void ReturnJob(IJob job) { }
}