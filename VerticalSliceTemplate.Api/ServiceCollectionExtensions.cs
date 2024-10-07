using FluentValidation;
using VerticalSliceTemplate.Api.Shared.Security;

namespace VerticalSliceTemplate.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(options =>
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services
            .AddSingleton(TimeProvider.System);

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    public static IServiceCollection RegisterPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((_, options) =>
        {
            string connectionString = configuration.GetConnectionString("Default")!;

            options.UseSqlite(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("ef_migration_history");
            });
        });

        return services;
    }

    public static async Task InitializeDatabase(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}