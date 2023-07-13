using AutoMapper;
using Helperx.Application.Services;
using Helperx.Infra.Data;
using Helperx.Infra.Data.Repository;
using Helperz.Application.Contracts;
using Helperz.Domain.Entities;
using Helperz.Domain.Interfaces.Repository;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Helperx.Api.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddControllers();

            serviceCollection.AddOptions();

            //Services
            serviceCollection.AddScoped<IHelperService, HelperService>();

            //Job Context
            serviceCollection.AddDbContext<JobContext>((options) => {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobRequest, Job>();
            });

            IMapper mapper = mapperConfig.CreateMapper();
            serviceCollection.AddSingleton(mapper);

            //Repository
            serviceCollection.AddScoped<IJobRepository, JobRepository>();

            serviceCollection.AddEndpointsApiExplorer();
;            //Swagger
            serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Helper X Api", Version = "v1" });

                // Configuração para incluir os comentários XML da documentação do projeto
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public static void UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHealthChecks("/health");
        }
    }
}
