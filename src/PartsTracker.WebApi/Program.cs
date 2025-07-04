
using Microsoft.EntityFrameworkCore;
using PartsTracker.Server.Data;
using PartsTracker.WebApi.Data;
using PartsTracker.WebApi.Infrastricture;

namespace PartsTracker.WebApi;

public class Program
{
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

        builder.Services.AddDbContextPool<InventoryDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

        builder.Services.AddHealthChecks().AddDbContextCheck<InventoryDbContext>();

        builder.Services.AddScoped<IPartsRepository, PartsRepository>();

        var app = builder.Build();

        app.MapHealthChecks("/health");

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
            db.Database.Migrate();
            await DbInitializer.SeedAsync(db);
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during migration or seeding.");
            throw;
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
