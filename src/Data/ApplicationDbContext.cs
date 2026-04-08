using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Prato> Pratos { get; set; }
        public DbSet<Ingrediente> Ingredientes { get; set; }
        public DbSet<PratoIngrediente> PratoIngredientes { get; set; }
        public DbSet<Cardapio> Cardapios { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<Atendimento> Atendimentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prato <-> Ingrediente
            modelBuilder.Entity<PratoIngrediente>()
                .HasKey(pi => new { pi.PratoId, pi.IngredienteId });

            modelBuilder.Entity<PratoIngrediente>()
                .HasOne(pi => pi.Prato)
                .WithMany(p => p.PratoIngredientes)
                .HasForeignKey(pi => pi.PratoId);

            modelBuilder.Entity<PratoIngrediente>()
                .HasOne(pi => pi.Ingrediente)
                .WithMany(i => i.PratoIngredientes)
                .HasForeignKey(pi => pi.IngredienteId);

            // Pedido -> Itens
            modelBuilder.Entity<ItemPedido>()
                .HasOne(i => i.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(i => i.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Pedido -> Usuario
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reserva -> Mesa
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Mesa)
                .WithMany()
                .HasForeignKey(r => r.MesaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reserva -> Usuario
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Endereco -> Usuario
            modelBuilder.Entity<Endereco>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
