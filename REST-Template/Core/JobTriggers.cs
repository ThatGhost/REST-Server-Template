using Backend.Jobs;
using Quartz;

// * * * * * <command to execute>
// | | | | |
// | | | | day of the week (0–6) (Sunday to Saturday; 
// | | | month (1–12)             7 is also Sunday on some systems)
// | | day of the month (1–31)
// | hour (0–23)
// minute (0–59)

namespace Backend.core
{
    public static class JobTriggers
    {
        public static void RegisterJobs(this IServiceCollection services)
        {
            services.AddQuartzHostedService();
            services.AddQuartz(quartz =>
            {
                // --Jobs--

                var jobKey = JobKey.Create(nameof(ExampleJob));
                quartz.AddJob<ExampleJob>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey)
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(7, 30))); // everyday at 7:30am
            });   
            
        }
    }
}
