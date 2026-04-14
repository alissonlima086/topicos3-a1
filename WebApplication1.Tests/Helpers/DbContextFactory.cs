using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Tests.Helpers
{
    // Cria o contexto para testes
    public static class DbContextFactory
    {
        public static ApplicationDbContext Criar(string? nomeBanco = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(nomeBanco ?? Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        // Cria usuario no contexto.
        public static async Task<Usuario> CriarUsuarioAsync(ApplicationDbContext ctx,
            string nome = "Teste", string email = "teste@email.com")
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                Nome = nome,
                SecurityStamp  = Guid.NewGuid().ToString()
            };
            ctx.Users.Add(usuario);
            await ctx.SaveChangesAsync();
            return usuario;
        }

        // Cria mesa no contexto.
        public static async Task<Mesa> CriarMesaAsync(ApplicationDbContext ctx,
            int numero = 1, bool disponivel = true)
        {
            var mesa = new Mesa { Numero = numero, Capacidade = 4, Disponivel = disponivel };
            ctx.Mesas.Add(mesa);
            await ctx.SaveChangesAsync();
            return mesa;
        }

        // Cria pedido no contexto.
        public static async Task<Pedido> CriarPedidoAsync(ApplicationDbContext ctx,
            Guid usuarioId, float precoUnitario = 20f, int quantidade = 1,
            bool foiSugestao = false)
        {
            var item = new ItemPedido
            {
                NomePrato = "Prato Teste",
                PrecoUnitario = precoUnitario,
                Quantidade = quantidade,
                FoiSugestao = foiSugestao
            };

            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                PrecoTotal = precoUnitario * quantidade,
                Itens = new List<ItemPedido> { item }
            };

            ctx.Pedidos.Add(pedido);
            await ctx.SaveChangesAsync();
            return pedido;
        }
    }
}
