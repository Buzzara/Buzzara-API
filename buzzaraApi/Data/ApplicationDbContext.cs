// Data/ApplicationDbContext.cs
using buzzaraApi.Models;
using Microsoft.EntityFrameworkCore;

namespace buzzaraApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Usuários e perfis
        public DbSet<Usuario> Usuarios { get; set; } = null!;

        public DbSet<PerfilAcompanhante> PerfisAcompanhantes { get; set; } = null!;

        // Agendamentos
        public DbSet<Agendamento> Agendamentos { get; set; } = null!;

        // Anúncios (Serviços)
        public DbSet<Servico> Servicos { get; set; } = null!;
        public DbSet<PagamentoAnuncio> PagamentosAnuncios { get; set; }

        // Mídia de perfil
        public DbSet<FotoAcompanhante> FotosAcompanhantes { get; set; } = null!;
        public DbSet<VideoAcompanhante> VideosAcompanhantes { get; set; } = null!;

        // Mídia de anúncio
        public DbSet<FotoAnuncio> FotosAnuncios { get; set; } = null!;
        public DbSet<VideoAnuncio> VideosAnuncios { get; set; } = null!;

        // Localização
        public DbSet<Localizacao> Localizacoes { get; set; } = null!;

        public DbSet<SobreUsuario> SobreUsuarios { get; set; } = null!;
        public DbSet<ServicoCache> ServicosCaches { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Servico>()
                .HasOne(s => s.Localizacao)
                .WithOne(l => l.Servico)
                .HasForeignKey<Localizacao>(l => l.ServicoID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.FotoPerfilUrl)
                .HasMaxLength(255);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.FotoCapaUrl)
                .HasMaxLength(255);

            // 1) Agendamento → PerfilAcompanhante (cascade ao excluir perfil)
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.PerfilAcompanhante)
                .WithMany(p => p.Agendamentos)
                .HasForeignKey(a => a.PerfilAcompanhanteID)
                .OnDelete(DeleteBehavior.Cascade);

            // 2) Agendamento → Usuario (cliente) sem cascade
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Cliente)
                .WithMany(u => u.Agendamentos)
                .HasForeignKey(a => a.ClienteID)
                .OnDelete(DeleteBehavior.NoAction);

            // 3) PerfilAcompanhante → Usuario sem cascade
            modelBuilder.Entity<PerfilAcompanhante>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.PerfisAcompanhantes)
                .HasForeignKey(p => p.UsuarioID)
                .OnDelete(DeleteBehavior.NoAction);

            // 4) FotoAcompanhante → PerfilAcompanhante (cascade ao excluir perfil)
            modelBuilder.Entity<FotoAcompanhante>()
                .HasOne(f => f.PerfilAcompanhante)
                .WithMany(p => p.Fotos)
                .HasForeignKey(f => f.PerfilAcompanhanteID)
                .OnDelete(DeleteBehavior.Cascade);

            // 5) VideoAcompanhante → PerfilAcompanhante (cascade ao excluir perfil)
            modelBuilder.Entity<VideoAcompanhante>()
                .HasOne(v => v.PerfilAcompanhante)
                .WithMany(p => p.Videos)
                .HasForeignKey(v => v.PerfilAcompanhanteID)
                .OnDelete(DeleteBehavior.Cascade);

            // 6) Servico (anúncio) → PerfilAcompanhante (cascade ao excluir perfil)
            modelBuilder.Entity<Servico>()
                .HasOne(s => s.PerfilAcompanhante)
                .WithMany(p => p.Servicos)
                .HasForeignKey(s => s.PerfilAcompanhanteID)
                .OnDelete(DeleteBehavior.Cascade);

            // 7) FotoAnuncio → Servico (cascade ao excluir anúncio)
            modelBuilder.Entity<FotoAnuncio>()
                .HasOne(f => f.Servico)
                .WithMany(s => s.Fotos)
                .HasForeignKey(f => f.ServicoID)
                .OnDelete(DeleteBehavior.Cascade);

            // 8) VideoAnuncio → Servico (cascade ao excluir anúncio)
            modelBuilder.Entity<VideoAnuncio>()
                .HasOne(v => v.Servico)
                .WithMany(s => s.Videos)
                .HasForeignKey(v => v.ServicoID)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
