using buzzaraApi.Models;
using Microsoft.EntityFrameworkCore;

namespace buzzaraApi.Data
{
    public static class DatabaseInitializationExtensions
    {
        public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILoggerFactory>()
                .CreateLogger("DatabaseInitialization");
            var context = services.GetRequiredService<ApplicationDbContext>();
            var configuration = services.GetRequiredService<IConfiguration>();

            const int maxAttempts = 10;
            const int retryDelaySeconds = 5;

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    logger.LogInformation("Applying database migrations. Attempt {Attempt} of {MaxAttempts}.", attempt, maxAttempts);
                    await context.Database.MigrateAsync();
                    await SeedAdminAsync(context, configuration, logger);
                    logger.LogInformation("Database migration and seed completed successfully.");
                    return;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    logger.LogWarning(ex, "Database initialization failed. Retrying in {DelaySeconds}s.", retryDelaySeconds);
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                }
            }

            await context.Database.MigrateAsync();
            await SeedAdminAsync(context, configuration, logger);
        }

        private static async Task SeedAdminAsync(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger logger)
        {
            var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@buzzara.com.br";
            var adminPassword = configuration["SeedAdmin:Password"] ?? "123456";
            var adminName = configuration["SeedAdmin:Name"] ?? "Administrador";
            var adminPhone = configuration["SeedAdmin:Phone"] ?? "11999999999";
            var adminCpf = configuration["SeedAdmin:Cpf"] ?? "00000000000";
            var adminGender = configuration["SeedAdmin:Gender"] ?? "Nao informado";

            var existingAdmin = await context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (existingAdmin != null)
            {
                logger.LogInformation("Seed admin user already exists.");
                return;
            }

            var admin = new Usuario
            {
                Nome = adminName,
                Email = adminEmail,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                Telefone = adminPhone,
                Cpf = adminCpf,
                Genero = adminGender,
                DataNascimento = new DateTime(2000, 1, 1),
                DataCadastro = DateTime.UtcNow,
                Role = "admin",
                IsValid = true,
                Ativo = true,
                EstaOnline = false
            };

            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();

            logger.LogInformation("Seed admin user created.");
        }
    }
}
