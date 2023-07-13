using AutoMapper;
using Helperx.Application.ConsumerServices;
using Helperx.Application.Services;
using Helperx.Infra.Data;
using Helperx.Infra.Data.Repository;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Helperz.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Program
{
    private static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddScoped<IJobRepository, JobRepository>();
                services.AddDbContext<JobContext>((options) =>
                {
                    options.UseSqlServer(context.Configuration.GetConnectionString("SqlServer"));
                });

                services.AddScoped<IHelperService, HelperService>();
                services.AddHostedService<ScheduledJobService>();

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<JobRequest, Job>();
                });

                services.AddScoped((c) => mapperConfig.CreateMapper());
            })
            .Build();

        await host.RunAsync();
    }
}