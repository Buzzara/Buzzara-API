using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using buzzaraApi.Data;
using Microsoft.EntityFrameworkCore;

public class UsuarioStatusService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public UsuarioStatusService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var limite = DateTime.Now.AddMinutes(-5);

            var usuariosInativos = await db.Usuarios
                .Where(u => u.EstaOnline && u.UltimoAcesso < limite)
                .ToListAsync();

            foreach (var u in usuariosInativos)
                u.EstaOnline = false;

            await db.SaveChangesAsync();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
        Console.WriteLine($"[{DateTime.Now}] Verificando usuários inativos...");
    }
}
