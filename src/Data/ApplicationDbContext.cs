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
        public DbSet<ItemCardapio> ItensCardapio { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<Atendimento> Atendimentos { get; set; }
        public DbSet<AtendimentoPresencial> AtendimentosPresenciais { get; set; }
        public DbSet<AtendimentoDeliveryProprio> AtendimentosDeliveryProprio { get; set; }
        public DbSet<AtendimentoDeliveryApp> AtendimentosDeliveryApp { get; set; }

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

            // Cardapio -> Itens
            modelBuilder.Entity<ItemCardapio>()
                .HasOne(ic => ic.Cardapio)
                .WithMany(c => c.Itens)
                .HasForeignKey(ic => ic.CardapioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemCardapio>()
                .HasOne(ic => ic.Prato)
                .WithMany()
                .HasForeignKey(ic => ic.PratoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cardapio: Data + Turno únicos
            modelBuilder.Entity<Cardapio>()
                .HasIndex(c => new { c.Data, c.Turno })
                .IsUnique();

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

            // Pedido -> Atendimento (1:1)
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Atendimento)
                .WithOne(a => a.Pedido)
                .HasForeignKey<Atendimento>(a => a.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Heranca
            modelBuilder.Entity<Atendimento>()
                .HasDiscriminator<string>("TipoAtendimento")
                .HasValue<AtendimentoPresencial>("Presencial")
                .HasValue<AtendimentoDeliveryProprio>("DeliveryProprio")
                .HasValue<AtendimentoDeliveryApp>("DeliveryApp");

            // AtendimentoPresencial -> Mesa
            modelBuilder.Entity<AtendimentoPresencial>()
                .HasOne(a => a.Mesa)
                .WithMany()
                .HasForeignKey(a => a.MesaId)
                .OnDelete(DeleteBehavior.Restrict);

            // AtendimentoDeliveryProprio -> Endereco
            modelBuilder.Entity<AtendimentoDeliveryProprio>()
                .HasOne(a => a.EnderecoEntrega)
                .WithMany()
                .HasForeignKey(a => a.EnderecoEntregaId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            modelBuilder.Entity<Atendimento>()
                .Property<Guid?>("MesaId").IsRequired(false);

            modelBuilder.Entity<Atendimento>()
                .Property<float?>("TaxaFixa").IsRequired(false);

            modelBuilder.Entity<Atendimento>()
                .Property<Guid?>("EnderecoEntregaId").IsRequired(false);

            modelBuilder.Entity<Atendimento>()
                .Property<string?>("NomeApp").IsRequired(false);

            modelBuilder.Entity<Atendimento>()
                .Property<float?>("ComissaoPorcentagem").IsRequired(false);

            modelBuilder.Entity<Atendimento>()
                .Property<float?>("TaxaAdicional").IsRequired(false);
        }
    }
}
