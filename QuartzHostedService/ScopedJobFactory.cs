using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Threading.Tasks;

namespace QuartzHostedServices
{
    public class ScopedJobFactory : IJobFactory
    {
        private readonly IServiceProvider _rootServiceProvider;

        public ScopedJobFactory(IServiceProvider rootServiceProvider)
        {
            _rootServiceProvider = rootServiceProvider ?? throw new ArgumentNullException(nameof(rootServiceProvider));
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobType = bundle.JobDetail.JobType;

            // MA - Generate a scope for the job, this allows the job to be registered
            //	using .AddScoped<T>() which means we can use scoped dependencies 
            //	e.g. database contexts
            var scope = _rootServiceProvider.CreateScope();

            var job = (IJob)scope.ServiceProvider.GetRequiredService(jobType);

            return new ScopedJob(scope, job);
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }


        private class ScopedJob : IJob, IDisposable
        {
            private readonly IServiceScope _scope;
            private readonly IJob _innerJob;

            public ScopedJob(IServiceScope scope, IJob innerJob)
            {
                _scope = scope;
                _innerJob = innerJob;
            }

            public Task Execute(IJobExecutionContext context) => _innerJob.Execute(context);


            public void Dispose()
            {
                (_innerJob as IDisposable)?.Dispose();
                _scope.Dispose();
            }
        }
    }
}
