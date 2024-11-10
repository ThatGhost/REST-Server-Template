using Backend.Services;
using Quartz;

namespace Backend.Jobs
{
    public class ExampleJob : IJob
    {
        // You can use Dependency Injection to get the services you require
        public ExampleJob()
        {

        }

        public async Task Execute(IJobExecutionContext context)
        {
            // Write your code here that you want to execute in the job
        }
    }
}
