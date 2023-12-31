using Helperx.Api.Configuration;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApiConfiguration(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        builder.Logging.AddConsole();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Helper-X API V1");
            c.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Run();
    }
}