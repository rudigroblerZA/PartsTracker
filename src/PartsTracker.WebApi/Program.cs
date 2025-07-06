
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PartsTracker.Server.Data;
using PartsTracker.WebApi.Data;
using PartsTracker.WebApi.Infrastricture;

namespace PartsTracker.WebApi;

/// <summary>
/// Main entry point for the PartsTracker Web API application.
/// </summary>
public class Program
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Program"/> class.
    /// </summary>
    protected Program()
    {
        
    }

    private static string _connectionString = String.Empty;

    /// <summary>
    /// Main method for the PartsTracker Web API application.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Once all tasks awaited.</returns>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(builder =>
        {
            builder.AddPolicy("AllowAllOrigins",
                policy => policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
        });


        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Version = "v1",
                Title = "PartsTracker",
                Description = "Mercedes-Benz's global factories rely on accurate, near-real-time visibility of the parts that flow \r\nthrough each production line. Your task is to build a thin vertical slice of a PartsTracker platform. "
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, "PartsTracker.WebApi.xml");
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        _connectionString = builder.Configuration.GetConnectionString("PostgresConnection") ??
            throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");

        builder.Services.AddDbContextPool<InventoryDbContext>(options => options.UseNpgsql(_connectionString));

        builder.Services.AddHealthChecks()
           .AddDbContextCheck<InventoryDbContext>()
           .AddNpgSql(
               _connectionString,
               name: "Postgres Database",
               failureStatus: HealthStatus.Unhealthy
           );

        builder.Services.AddScoped<IPartsRepository, PartsRepository>();

        var app = builder.Build();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseCors("AllowAllOrigins");

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PartsTracker API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            await db.Database.MigrateAsync();
            await DbInitializer.SeedAsync(db);
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during migration or seeding.");
        }

        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        await app.RunAsync();
    }
}
