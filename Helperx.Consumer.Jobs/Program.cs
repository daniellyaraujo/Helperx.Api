using Helperx.Application.ConsumerServices;
using Helperx.Application.Services;
using Helperx.Infra.Data;
using Helperx.Infra.Data.Repository;
using Helperz.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddScoped<IJobRepository, JobRepository>();
                services.AddDbContext<JobContext>((options) => {
                    options.UseSqlServer(context.Configuration.GetConnectionString("SqlServer"));
                });
                services.AddScoped<IHelperService, HelperService>();
                services.AddHostedService<ScheduledJobService>();
            })
            .Build();

await host.RunAsync();