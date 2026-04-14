using Xunit;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Models.ViewModel;
using WebApplication1.Services;
using WebApplication1.Tests.Helpers;
using WebApplication1.Data;

namespace WebApplication1.Tests.Services
{
    public class RelatorioServiceTests
    {

        private static async Task<(ApplicationDbContext ctx, Guid usuarioId)> Setup()
        {
            var ctx = DbContextFactory.Criar();
            var usuario = await DbContextFactory.CriarUsuarioAsync(ctx);
            return (ctx, usuario.Id);
        }

        private static Pedido NovoPedido(Guid usuarioId, DateTime dataHora,
            float precoTotal, Status status = Status.Entregue) => new()
        {
            UsuarioId  = usuarioId,
            DataHora   = dataHora,
            PrecoTotal = precoTotal,
            Status     = status
        };

        [Fact]
        public async Task GerarAsync_SemPedidos_RetornaZerado()
        {
            var (ctx, _) = await Setup();
            await using (ctx)
            {
                var service   = new RelatorioService(ctx);
                var resultado = await service.GerarAsync(PeriodoRelatorio.Mes);

                Assert.Equal(0f, resultado.FaturamentoTotal);
                Assert.Equal(0, resultado.TotalPedidos);
                Assert.Empty(resultado.FaturamentoPorTipo);
            }
        }

        [Fact]
        public async Task GerarAsync_PedidosDeliveryProprio_ContabilizaFaturamento()
        {
            var (ctx, usuarioId) = await Setup();
            await using (ctx)
            {
                var pedido = NovoPedido(usuarioId, DateTime.Today, 100f);
                pedido.Itens.Add(new ItemPedido
                    { NomePrato = "X-Burguer", PrecoUnitario = 100f, Quantidade = 1 });
                ctx.Pedidos.Add(pedido);
                await ctx.SaveChangesAsync();

                ctx.AtendimentosDeliveryProprio.Add(
                    new AtendimentoDeliveryProprio(5f) { PedidoId = pedido.Id, Status = Status.Finalizado });
                await ctx.SaveChangesAsync();

                var service   = new RelatorioService(ctx);
                var resultado = await service.GerarAsync(PeriodoRelatorio.Mes);

                Assert.Equal(100f, resultado.FaturamentoTotal);
                Assert.Equal(1, resultado.TotalPedidos);
                Assert.Single(resultado.FaturamentoPorTipo);
                Assert.Equal("Delivery Próprio", resultado.FaturamentoPorTipo[0].TipoAtendimento);
            }
        }

        [Fact]
        public async Task GerarAsync_PedidosDeliveryApp_AgrupaPorNomeApp()
        {
            var (ctx, usuarioId) = await Setup();
            await using (ctx)
            {
                // Pedido iFood
                var p1 = NovoPedido(usuarioId, DateTime.Today, 80f);
                p1.Itens.Add(new ItemPedido { NomePrato = "Pizza", PrecoUnitario = 80f, Quantidade = 1 });
                // Pedido Rappi
                var p2 = NovoPedido(usuarioId, DateTime.Today, 60f);
                p2.Itens.Add(new ItemPedido { NomePrato = "Sushi", PrecoUnitario = 60f, Quantidade = 1 });
                // Segundo pedido iFood
                var p3 = NovoPedido(usuarioId, DateTime.Today, 50f);
                p3.Itens.Add(new ItemPedido { NomePrato = "Burguer", PrecoUnitario = 50f, Quantidade = 1 });

                ctx.Pedidos.AddRange(p1, p2, p3);
                await ctx.SaveChangesAsync();

                ctx.AtendimentosDeliveryApp.Add(
                    new AtendimentoDeliveryApp("iFood", 0.12f, 2.5f) { PedidoId = p1.Id, Status = Status.Finalizado });
                ctx.AtendimentosDeliveryApp.Add(
                    new AtendimentoDeliveryApp("Rappi", 0.15f, 3f) { PedidoId = p2.Id, Status = Status.Finalizado });
                ctx.AtendimentosDeliveryApp.Add(
                    new AtendimentoDeliveryApp("iFood", 0.12f, 2.5f) { PedidoId = p3.Id, Status = Status.Finalizado });
                await ctx.SaveChangesAsync();

                var service = new RelatorioService(ctx);
                var resultado = await service.GerarAsync(PeriodoRelatorio.Mes);

                // iFood e Rappi
                Assert.Equal(2, resultado.FaturamentoPorTipo.Count);

                var ifood = resultado.FaturamentoPorTipo.First(f => f.TipoAtendimento == "iFood");
                Assert.Equal(2, ifood.Quantidade);
                Assert.Equal(130f, ifood.Total);

                var rappi = resultado.FaturamentoPorTipo.First(f => f.TipoAtendimento == "Rappi");
                Assert.Equal(1, rappi.Quantidade);
                Assert.Equal(60f, rappi.Total);
            }
        }

        [Fact]
        public async Task GerarAsync_AtendimentoPresencialFinalizado_Contabiliza()
        {
            var (ctx, usuarioId) = await Setup();
            await using (ctx)
            {
                var mesa = await DbContextFactory.CriarMesaAsync(ctx);

                var pedido = NovoPedido(usuarioId, DateTime.Today, 120f, Status.EmAtendimento);
                pedido.Itens.Add(new ItemPedido
                    { NomePrato = "Filé", PrecoUnitario = 120f, Quantidade = 1 });
                ctx.Pedidos.Add(pedido);
                await ctx.SaveChangesAsync();

                ctx.AtendimentosPresenciais.Add(new AtendimentoPresencial(mesa.Id)
                {
                    PedidoId = pedido.Id,
                    Status = Status.Finalizado
                });
                await ctx.SaveChangesAsync();

                var service = new RelatorioService(ctx);
                var resultado = await service.GerarAsync(PeriodoRelatorio.Mes);

                Assert.Equal(120f, resultado.FaturamentoTotal);
                Assert.Equal("Presencial (Mesa)", resultado.FaturamentoPorTipo[0].TipoAtendimento);
            }
        }

        [Fact]
        public async Task GerarAsync_AtendimentoPresencialNaoFinalizado_NaoContabiliza()
        {
            var (ctx, usuarioId) = await Setup();
            await using (ctx)
            {
                var mesa = await DbContextFactory.CriarMesaAsync(ctx);
                var pedido = NovoPedido(usuarioId, DateTime.Today, 90f, Status.EmAtendimento);
                pedido.Itens.Add(new ItemPedido
                    { NomePrato = "Massa", PrecoUnitario = 90f, Quantidade = 1 });
                ctx.Pedidos.Add(pedido);
                await ctx.SaveChangesAsync();

                ctx.AtendimentosPresenciais.Add(new AtendimentoPresencial(mesa.Id)
                {
                    PedidoId = pedido.Id,
                    Status = Status.EmAtendimento   // ainda em aberto
                });
                await ctx.SaveChangesAsync();

                var service = new RelatorioService(ctx);
                var resultado = await service.GerarAsync(PeriodoRelatorio.Mes);

                Assert.Equal(0f, resultado.FaturamentoTotal);
                Assert.Empty(resultado.FaturamentoPorTipo);
            }
        }

        [Fact]
        public async Task GerarAsync_ItensMaisVendidos_OrdenaDescendente()
        {
            var (ctx, usuarioId) = await Setup();
            await using (ctx)
            {
                var p1 = NovoPedido(usuarioId, DateTime.Today, 0f);
                p1.Itens.Add(new ItemPedido { NomePrato = "A", PrecoUnitario = 10f, Quantidade = 5, FoiSugestao = false });
                p1.Itens.Add(new ItemPedido { NomePrato = "B", PrecoUnitario = 10f, Quantidade = 2, FoiSugestao = false });
                ctx.Pedidos.Add(p1);
                await ctx.SaveChangesAsync();

                ctx.AtendimentosDeliveryProprio.Add(
                    new AtendimentoDeliveryProprio(0f) { PedidoId = p1.Id, Status = Status.Finalizado });
                await ctx.SaveChangesAsync();

                var service = new RelatorioService(ctx);
                var resultado = await service.GerarAsync(PeriodoRelatorio.Mes);

                Assert.Equal("A", resultado.ItensMaisVendidos[0].NomePrato);
                Assert.Equal("B", resultado.ItensMaisVendidos[1].NomePrato);
            }
        }

        [Fact]
        public async Task GerarAsync_SugestoesSeparadasDosItensRegulares()
        {
            var (ctx, usuarioId) = await Setup();
            await using (ctx)
            {
                var p1 = NovoPedido(usuarioId, DateTime.Today, 0f);
                p1.Itens.Add(new ItemPedido { NomePrato = "Regular", PrecoUnitario = 20f, Quantidade = 3, FoiSugestao = false });
                p1.Itens.Add(new ItemPedido { NomePrato = "Sugestão Chef", PrecoUnitario = 30f, Quantidade = 2, FoiSugestao = true });
                ctx.Pedidos.Add(p1);
                await ctx.SaveChangesAsync();

                ctx.AtendimentosDeliveryProprio.Add(
                    new AtendimentoDeliveryProprio(0f) { PedidoId = p1.Id, Status = Status.Finalizado });
                await ctx.SaveChangesAsync();

                var service = new RelatorioService(ctx);
                var resultado = await service.GerarAsync(PeriodoRelatorio.Mes);

                Assert.Single(resultado.ItensMaisVendidos);
                Assert.Equal("Regular", resultado.ItensMaisVendidos[0].NomePrato);

                Assert.Single(resultado.SugestoesMaisVendidas);
                Assert.Equal("Sugestão Chef", resultado.SugestoesMaisVendidas[0].NomePrato);
            }
        }

        [Fact]
        public async Task GerarAsync_FiltroPorIntervaloCustomizado_IgnoraPedidosForaDoPeriodo()
        {
            var (ctx, usuarioId) = await Setup();
            await using (ctx)
            {
                // Dentro do período
                var pDentro = NovoPedido(usuarioId, DateTime.Today.AddDays(-5), 200f);
                pDentro.Itens.Add(new ItemPedido { NomePrato = "X", PrecoUnitario = 200f, Quantidade = 1 });

                // Fora do período
                var pFora = NovoPedido(usuarioId, DateTime.Today.AddDays(-30), 500f);
                pFora.Itens.Add(new ItemPedido { NomePrato = "Y", PrecoUnitario = 500f, Quantidade = 1 });

                ctx.Pedidos.AddRange(pDentro, pFora);
                await ctx.SaveChangesAsync();

                ctx.AtendimentosDeliveryProprio.Add(
                    new AtendimentoDeliveryProprio(0f) { PedidoId = pDentro.Id, Status = Status.Finalizado });
                ctx.AtendimentosDeliveryProprio.Add(
                    new AtendimentoDeliveryProprio(0f) { PedidoId = pFora.Id, Status = Status.Finalizado });
                await ctx.SaveChangesAsync();

                var service = new RelatorioService(ctx);
                var resultado = await service.GerarAsync(
                    PeriodoRelatorio.Mes,
                    dataInicio: DateTime.Today.AddDays(-7),
                    dataFim: DateTime.Today);

                Assert.Equal(200f, resultado.FaturamentoTotal);
                Assert.Equal(1, resultado.TotalPedidos);
            }
        }
    }
}
