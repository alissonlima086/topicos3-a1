using Xunit;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Tests.Helpers;

namespace WebApplication1.Tests.Services
{
    public class ItemPedidoServiceTests
    {

        [Fact]
        public async Task AdicionarAsync_PedidoValido_RetornaItemAdicionado()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var pedido = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id);
            var service = new ItemPedidoService(ctx);

            var item = new ItemPedido
            {
                PedidoId = pedido.Id,
                NomePrato = "Sopa do Dia",
                PrecoUnitario = 18f,
                Quantidade = 1
            };

            var (resultado, erro) = await service.AdicionarAsync(item);

            Assert.Null(erro);
            Assert.NotNull(resultado);
            Assert.NotEqual(Guid.Empty, resultado.Id);
        }

        [Fact]
        public async Task AdicionarAsync_PedidoInexistente_RetornaErro()
        {
            await using var ctx = DbContextFactory.Criar();
            var service = new ItemPedidoService(ctx);

            var item = new ItemPedido
            {
                PedidoId = Guid.NewGuid(),
                NomePrato = "Qualquer",
                PrecoUnitario = 10f,
                Quantidade = 1
            };

            var (resultado, erro) = await service.AdicionarAsync(item);

            Assert.Null(resultado);
            Assert.Equal("Pedido não encontrado.", erro);
        }

        [Fact]
        public async Task AdicionarAsync_QuantidadeZero_RetornaErro()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var pedido = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id);
            var service = new ItemPedidoService(ctx);

            var item = new ItemPedido
            {
                PedidoId = pedido.Id,
                NomePrato = "Prato X",
                PrecoUnitario = 20f,
                Quantidade = 0
            };

            var (resultado, erro) = await service.AdicionarAsync(item);

            Assert.Null(resultado);
            Assert.Equal("Quantidade deve ser maior que zero.", erro);
        }

        [Fact]
        public async Task AdicionarAsync_AtualizaPrecoTotalDoPedido()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var pedido = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id, precoUnitario: 30f, quantidade: 1);
            var precoAntes = pedido.PrecoTotal; // 30
            var service= new ItemPedidoService(ctx);

            var item = new ItemPedido
            {
                PedidoId = pedido.Id,
                NomePrato = "Sobremesa",
                PrecoUnitario = 15f,
                Quantidade = 2
            };

            await service.AdicionarAsync(item);

            var pedidoAtualizado = await ctx.Pedidos.FindAsync(pedido.Id);
            // 30 (existente) + 15*2 (novo) = 60
            Assert.Equal(precoAntes + 30f, pedidoAtualizado!.PrecoTotal);
        }

        [Fact]
        public async Task RemoverAsync_ItemExistente_RemoveEAjustaPreco()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var pedido = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id, precoUnitario: 50f, quantidade: 1);
            var service = new ItemPedidoService(ctx);

            var item = pedido.Itens.First();

            var resultado = await service.RemoverAsync(item.Id);

            Assert.True(resultado);
            Assert.False(service.Existe(item.Id));

            var pedidoAtualizado = await ctx.Pedidos.FindAsync(pedido.Id);
            Assert.Equal(0f, pedidoAtualizado!.PrecoTotal);
        }

        [Fact]
        public async Task RemoverAsync_ItemInexistente_RetornaFalse()
        {
            await using var ctx = DbContextFactory.Criar();
            var service = new ItemPedidoService(ctx);

            var resultado = await service.RemoverAsync(Guid.NewGuid());

            Assert.False(resultado);
        }

        [Fact]
        public async Task ListarPorPedidoAsync_RetornaSoItensDoPedido()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var pedido1 = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id);
            var pedido2 = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id);
            var service = new ItemPedidoService(ctx);

            var itens = await service.ListarPorPedidoAsync(pedido1.Id);

            Assert.All(itens, i => Assert.Equal(pedido1.Id, i.PedidoId));
        }

        [Fact]
        public async Task BuscarPorIdAsync_IdExistente_RetornaItem()
        {
            await using var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            var pedido  = await DbContextFactory.CriarPedidoAsync(ctx, usuario.Id);
            var item = pedido.Itens.First();
            var service = new ItemPedidoService(ctx);

            var resultado = await service.BuscarPorIdAsync(item.Id);

            Assert.NotNull(resultado);
            Assert.Equal(item.Id, resultado.Id);
        }

        [Fact]
        public async Task BuscarPorIdAsync_IdInexistente_RetornaNull()
        {
            await using var ctx = DbContextFactory.Criar();
            var service = new ItemPedidoService(ctx);

            var resultado = await service.BuscarPorIdAsync(Guid.NewGuid());

            Assert.Null(resultado);
        }
    }
}
