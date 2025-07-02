
using Microsoft.EntityFrameworkCore;
using PartsTracker.Infrastructure;

namespace PartsTracker;

/// <summary>
/// The main entry point for the PartsTracker application.
/// </summary>
public class Program
{
    /// <summary>
    /// The main method that runs the PartsTracker application.
    /// </summary>
    /// <param name="args">Command line args</param>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "v1",
                Title = "PartsTracker",
                Description = "Mercedes-Benz's global factories rely on accurate, near-real-time visibility of the parts that flow \r\nthrough each production line. Your task is to build a thin vertical slice of a PartsTracker platform. "
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, "PartsTracker.xml");
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        builder.Services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<InventoryDbContext>();

        builder.Services.AddScoped<IPartsRepository, PartsRepository>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        var app = builder.Build();

        app.MapHealthChecks("/health");

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
            db.Database.Migrate();
            await DbInitializer.SeedAsync(db);
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during migration or seeding.");
            throw;
        }

        app.UseCors("AllowAll");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
