using Helper.Domain.Interfaces.Repository;
using Helperx.Application.Services;
using Helperx.Infra.Data;
using Helperx.Infra.Data.Repository;
using Microsoft.Azure.ServiceBus;
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

            serviceCollection.AddSingleton(serviceProvider =>
            {
                string connectionString = configuration["ServiceBus:ConnectionString"];
                return new QueueClient(connectionString, "NomeDaFila");
            });

            serviceCollection.AddSignalR();

            //serviceCollection.AddDbContext<JobContext>(
            //    options => options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

            //serviceCollection.AddHealthChecks()
            //                  .AddSqlServer(connectionString: configuration.GetConnectionString("SqlServer")!,
            //                     name: "SQL Server Instance");

            //Services
            serviceCollection.AddScoped<IHelperService, HelperService>();
            serviceCollection.AddScoped<IQueueSenderService, QueueSenderService>();
            serviceCollection.AddScoped<IHelperService, HelperService>();

            //Job Context
            serviceCollection.AddSingleton<JobContext>();

            //Repository
            serviceCollection.AddScoped<IJobRepository, JobRepository>();

            //Swagger
            serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nome da API", Version = "v1" });

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
