using Xunit;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Services;
using WebApplication1.Tests.Helpers;

namespace WebApplication1.Tests.Services
{
    public class PedidoServiceTests
    {

        [Fact]
        public async Task CriarAsync_UsuarioValido_RetornaPedidoCriado()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var service = new PedidoService(ctx);

            var pedido = new Pedido
            {
                UsuarioId = usuario.Id,
                Itens = new List<ItemPedido>
                {
                    new() { NomePrato = "Frango Grelhado", PrecoUnitario = 35f, Quantidade = 2 }
                }
            };

            var (resultado, erro) = await service.CriarAsync(pedido);

            Assert.Null(erro);
            Assert.NotNull(resultado);
            Assert.NotEqual(Guid.Empty, resultado.Id);
        }

        [Fact]
        public async Task CriarAsync_UsuarioInexistente_RetornaErro()
        {
            await using var ctx = DbContextFactory.Criar();
            var service = new PedidoService(ctx);

            var pedido = new Pedido { UsuarioId = Guid.NewGuid() };

            var (resultado, erro) = await service.CriarAsync(pedido);

            Assert.Null(resultado);
            Assert.Equal("Usuário não encontrado.", erro);
        }

        [Fact]
        public async Task CriarAsync_SemPrecoTotal_CalculaAutomaticamente()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var service = new PedidoService(ctx);

            var pedido = new Pedido
            {
                UsuarioId   = usuario.Id,
                TaxaEntrega = 5f,
                PrecoTotal  = 0f,
                Itens = new List<ItemPedido>
                {
                    new() { NomePrato = "Risoto", PrecoUnitario = 40f, Quantidade = 2 }
                }
            };

            var (resultado, _) = await service.CriarAsync(pedido);

            // 40 * 2 + 5 (taxa) = 85
            Assert.Equal(85f, resultado!.PrecoTotal);
        }

        [Fact]
        public async Task CriarAsync_StatusDefault_DefinePendente()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var service = new PedidoService(ctx);

            var pedido = new Pedido { UsuarioId = usuario.Id };

            var (resultado, _) = await service.CriarAsync(pedido);

            Assert.Equal(Status.Pendente, resultado!.Status);
        }

        [Fact]
        public async Task CriarAsync_ItensPedido_RecebenPedidoIdCorreto()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var service = new PedidoService(ctx);

            var item = new ItemPedido { NomePrato = "Salada", PrecoUnitario = 15f, Quantidade = 1 };
            var pedido = new Pedido { UsuarioId = usuario.Id, Itens = new List<ItemPedido> { item } };

            var (resultado, _) = await service.CriarAsync(pedido);

            Assert.Equal(resultado!.Id, resultado.Itens.First().PedidoId);
        }

        [Fact]
        public async Task BuscarPorIdAsync_IdExistente_RetornaPedido()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var pedido = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id);
            var service = new PedidoService(ctx);

            var resultado = await service.BuscarPorIdAsync(pedido.Id);

            Assert.NotNull(resultado);
            Assert.Equal(pedido.Id, resultado.Id);
        }

        [Fact]
        public async Task BuscarPorIdAsync_IdInexistente_RetornaNull()
        {
            await using var ctx = DbContextFactory.Criar();
            var service = new PedidoService(ctx);

            var resultado = await service.BuscarPorIdAsync(Guid.NewGuid());

            Assert.Null(resultado);
        }

        [Fact]
        public async Task AtualizarStatusAsync_PedidoExistente_AtualizaStatus()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var pedido = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id);
            var service = new PedidoService(ctx);

            var resultado = await service.AtualizarStatusAsync(pedido.Id, Status.Confirmado);

            Assert.NotNull(resultado);
            Assert.Equal(Status.Confirmado, resultado.Status);
        }

        [Fact]
        public async Task AtualizarStatusAsync_IdInexistente_RetornaNull()
        {
            await using var ctx = DbContextFactory.Criar();
            var service = new PedidoService(ctx);

            var resultado = await service.AtualizarStatusAsync(Guid.NewGuid(), Status.Cancelado);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ExcluirAsync_PedidoExistente_RemoveDoContexto()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var pedido = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id);
            var service = new PedidoService(ctx);

            var resultado = await service.ExcluirAsync(pedido.Id);

            Assert.True(resultado);
            Assert.False(service.Existe(pedido.Id));
        }

        [Fact]
        public async Task ExcluirAsync_IdInexistente_RetornaFalse()
        {
            await using var ctx = DbContextFactory.Criar();
            var service = new PedidoService(ctx);

            var resultado = await service.ExcluirAsync(Guid.NewGuid());

            Assert.False(resultado);
        }

        [Fact]
        public async Task ListarPorUsuarioAsync_RetornaSoPedidosDoUsuario()
        {
            await using var ctx = DbContextFactory.Criar();
            var u1 = await DbContextFactory.CriarUsuarioAsync(ctx, "User1", "u1@email.com");
            var u2 = await DbContextFactory.CriarUsuarioAsync(ctx, "User2", "u2@email.com");
            await DbContextFactory.CriarPedidoAsync(ctx, u1.Id);
            await DbContextFactory.CriarPedidoAsync(ctx, u1.Id);
            await DbContextFactory.CriarPedidoAsync(ctx, u2.Id);
            var service = new PedidoService(ctx);

            var resultado = await service.ListarPorUsuarioAsync(u1.Id);

            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, p => Assert.Equal(u1.Id, p.UsuarioId));
        }
    }
}
