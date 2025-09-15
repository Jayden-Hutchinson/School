using CytoNET.Data.Protein;
using Cytonet.Data.ProteinInteraction;
using CytoNET.Data.ProteinModification;
using CytoNET.Data.SmallMolecule;
using CytoNET.Data.TissueDistribution;
using CytoNET.Repository;
using CytoNET.Repository.Interfaces;
using CytoNET.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddAzureWebAppDiagnostics();

var loggerFactory = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddDebug();
});
var logger = loggerFactory.CreateLogger<Program>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProteinModificationRepository, ProteinModificationRepository>();

// builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

builder.Services.AddScoped<IProteinModelRepository, ProteinModelRepository>();

builder.Services.AddScoped<ITissueDistributionRepository, TissueDistributionRepository>();
builder.Services.AddScoped<IProteinInteractionRepository, ProteinInteractionRepository>();

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<ProteinDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("ProteinConnection"))
    );

    builder.Services.AddDbContext<ProteinModificationDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("ProteinModificationConnection")
        )
    );

    builder.Services.AddDbContext<ProteinInteractionDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("ProteinInteractionConnection"))
    );

    builder.Services.AddDbContext<SmallMoleculeDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("SmallMoleculeConnection"))
    );

    builder.Services.AddDbContext<TissueDistributionDbContext>(options =>
        options
            .UseSqlite(builder.Configuration.GetConnectionString("TissueDistributionConnection"))
            .EnableSensitiveDataLogging()
    );
}
else
{
    // Add db contexts
    builder.Services.AddDbContext<ProteinDbContext>(options =>
        options
            .UseSqlite(builder.Configuration.GetConnectionString("ProteinConnection"))
            .EnableSensitiveDataLogging()
    );

    builder.Services.AddDbContext<ProteinModificationDbContext>(options =>
        options
            .UseSqlite(builder.Configuration.GetConnectionString("ProteinModificationConnection"))
            .EnableSensitiveDataLogging()
    );

    builder.Services.AddDbContext<ProteinInteractionDbContext>(options =>
        options
            .UseSqlite(builder.Configuration.GetConnectionString("ProteinInteractionConnection"))
            .EnableSensitiveDataLogging()
    );

    builder.Services.AddDbContext<SmallMoleculeDbContext>(options =>
        options
            .UseSqlite(builder.Configuration.GetConnectionString("SmallMoleculeConnection"))
            .EnableSensitiveDataLogging()
    );

    builder.Services.AddDbContext<TissueDistributionDbContext>(options =>
        options
            .UseSqlite(builder.Configuration.GetConnectionString("TissueDistributionConnection"))
            .EnableSensitiveDataLogging()
    );
}

// Run these commands in the terminal to create the database and tables

/*
# For all databases at once
chmod +x Scripts/run-migrations.sh
./Scripts/run-migrations.sh

# For ProteinDbContext
dotnet ef migrations add InitialProteinCreate --context ProteinDbContext
dotnet ef database update --context ProteinDbContext

# For ProteinModificationDbContext
dotnet ef migrations add InitialProteinModificationCreate --context ProteinModificationDbContext
dotnet ef database update --context ProteinModificationDbContext

# For ProteinInteractionDbContext
dotnet ef migrations add InitialProteinInteractionCreate --context ProteinInteractionDbContext
dotnet ef database update --context ProteinInteractionDbContext

# For SmallMoleculeDbContext
dotnet ef migrations add InitialSmallMoleculeCreate --context SmallMoleculeDbContext
dotnet ef database update --context SmallMoleculeDbContext

# For TissueDistributionDbContext
dotnet ef migrations add InitialTissueDistributionCreate --context TissueDistributionDbContext
dotnet ef database update --context TissueDistributionDbContext
*/

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    string seedDataDirectory = Path.Combine(app.Environment.ContentRootPath, "SeedData");
    logger.LogInformation("Using seed data directory: {SeedDataDirectory}", seedDataDirectory);

    var cleaner = new CsvCleaner(seedDataDirectory);
    cleaner.CleanCsvFiles();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        var services = scope.ServiceProvider;
        logger.LogWarning(
            "Starting database initialization in {Environment} environment",
            app.Environment.EnvironmentName
        );

        // Get all database contexts
        var smallMoleculeContext = services.GetRequiredService<SmallMoleculeDbContext>();
        var proteinContext = services.GetRequiredService<ProteinDbContext>();
        var proteinModificationContext =
            services.GetRequiredService<ProteinModificationDbContext>();
        var proteinInteractionContext = services.GetRequiredService<ProteinInteractionDbContext>();
        var tissueDistributionContext = services.GetRequiredService<TissueDistributionDbContext>();

        // Apply migrations
        logger.LogWarning(
            "Applying database migrations in {env} environment...",
            app.Environment.EnvironmentName
        );

        smallMoleculeContext.Database.Migrate();
        proteinContext.Database.Migrate();
        proteinModificationContext.Database.Migrate();
        proteinInteractionContext.Database.Migrate();
        tissueDistributionContext.Database.Migrate();

        logger.LogWarning("Database migrations completed");

        //// Count records to check database state
        var proteinCount = proteinContext.Proteins.Count();
        var interactionCount = proteinInteractionContext.ProteinInteractions.Count();
        logger.LogWarning(
            "Database counts: Proteins={proteins}, Interactions={interactions}",
            proteinCount,
            interactionCount
        );

        //// Only seed if empty (works in both environments)
        if (proteinCount == 0)
        {
            string seedDataDirectory = Path.Combine(app.Environment.ContentRootPath, "SeedData");
            logger.LogWarning(
                "Database is empty. Looking for seed data in: {dir}",
                seedDataDirectory
            );

            if (Directory.Exists(seedDataDirectory))
            {
                logger.LogWarning("Found seed directory, starting database seeding");

                // List seed files to help with debugging
                string[] seedFiles = Directory.GetFiles(seedDataDirectory, "*.csv");
                logger.LogWarning("Found {count} seed files", seedFiles.Length);

                DatabaseSeeder.SeedAllDatabases(
                    smallMoleculeContext,
                    proteinContext,
                    proteinModificationContext,
                    proteinInteractionContext,
                    tissueDistributionContext
                );

                logger.LogWarning("Database seeding completed");

                // Verify seeding worked
                logger.LogWarning(
                    "Database counts after seeding: Proteins={proteins}, Interactions={interactions}",
                    proteinContext.Proteins.Count(),
                    proteinInteractionContext.ProteinInteractions.Count()
                );
            }
            else
            {
                logger.LogWarning("Seed data directory not found: {dir}", seedDataDirectory);
            }
        }
        else
        {
            logger.LogWarning("Database already contains data, skipping seeding");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapGet(
    "/api/diag/dbstatus",
    async (ProteinDbContext proteinContext, ProteinInteractionDbContext interactionContext) =>
    {
        return new
        {
            ProteinCount = await proteinContext.Proteins.CountAsync(),
            InteractionCount = await interactionContext.ProteinInteractions.CountAsync(),
        };
    }
);

app.Run();
