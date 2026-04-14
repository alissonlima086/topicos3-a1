using Xunit;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Services;
using WebApplication1.Tests.Helpers;

namespace WebApplication1.Tests.Services
{
    public class AtendimentoServiceTests
    {

        [Fact]
        public async Task CriarPresencialAsync_MesaDisponivel_CriaAtendimentoEOcupaMesa()
        {
            await using var ctx = DbContextFactory.Criar();
            await DbContextFactory.CriarUsuarioAsync(ctx);
            var mesa = await DbContextFactory.CriarMesaAsync(ctx, numero: 1, disponivel: true);
            var service = new AtendimentoService(ctx);

            var (atendimento, erro) = await service.CriarPresencialAsync(mesa.Id);

            Assert.Null(erro);
            Assert.NotNull(atendimento);

            var mesaAtualizada = await ctx.Mesas.FindAsync(mesa.Id);
            Assert.False(mesaAtualizada!.Disponivel);
        }

        [Fact]
        public async Task CriarPresencialAsync_MesaOcupada_RetornaErro()
        {
            await using var ctx = DbContextFactory.Criar();
            await DbContextFactory.CriarUsuarioAsync(ctx);
            var mesa = await DbContextFactory.CriarMesaAsync(ctx, numero: 2, disponivel: false);
            var service = new AtendimentoService(ctx);

            var (atendimento, erro) = await service.CriarPresencialAsync(mesa.Id);

            Assert.Null(atendimento);
            Assert.Contains("ocupada", erro);
        }

        [Fact]
        public async Task CriarPresencialAsync_MesaInexistente_RetornaErro()
        {
            await using var ctx = DbContextFactory.Criar();
            var service = new AtendimentoService(ctx);

            var (atendimento, erro) = await service.CriarPresencialAsync(Guid.NewGuid());

            Assert.Null(atendimento);
            Assert.Equal("Mesa não encontrada.", erro);
        }

        [Fact]
        public async Task CriarPresencialAsync_SemUsuarioNoBanco_RetornaErro()
        {
            await using var ctx = DbContextFactory.Criar();
            var mesa = await DbContextFactory.CriarMesaAsync(ctx);
            var service = new AtendimentoService(ctx);

            var (atendimento, erro) = await service.CriarPresencialAsync(mesa.Id);

            Assert.Null(atendimento);
            Assert.Equal("Nenhum usuário cadastrado no sistema.", erro);
        }

        [Fact]
        public async Task CriarPresencialAsync_CriaComStatusEmAtendimento()
        {
            await using var ctx = DbContextFactory.Criar();
            await DbContextFactory.CriarUsuarioAsync(ctx);
            var mesa = await DbContextFactory.CriarMesaAsync(ctx);
            var service = new AtendimentoService(ctx);

            var (atendimento, _) = await service.CriarPresencialAsync(mesa.Id);

            Assert.Equal(Status.EmAtendimento, atendimento!.Status);
        }

        [Fact]
        public async Task FinalizarAsync_AtendimentoExistente_FinalizaELiberaMesa()
        {
            await using var ctx = DbContextFactory.Criar();
            await DbContextFactory.CriarUsuarioAsync(ctx);
            var mesa = await DbContextFactory.CriarMesaAsync(ctx);
            var service = new AtendimentoService(ctx);

            var (atendimento, _) = await service.CriarPresencialAsync(mesa.Id);
            var resultado = await service.FinalizarAsync(atendimento!.Id);

            Assert.True(resultado);

            var mesaAtualizada = await ctx.Mesas.FindAsync(mesa.Id);
            Assert.True(mesaAtualizada!.Disponivel);

            var atendimentoAtualizado = await ctx.AtendimentosPresenciais.FindAsync(atendimento.Id);
            Assert.Equal(Status.Finalizado, atendimentoAtualizado!.Status);
        }

        [Fact]
        public async Task FinalizarAsync_IdInexistente_RetornaFalse()
        {
            await using var ctx = DbContextFactory.Criar();
            var service = new AtendimentoService(ctx);

            var resultado = await service.FinalizarAsync(Guid.NewGuid());

            Assert.False(resultado);
        }

        [Fact]
        public async Task ListarPresenciaisAsync_RetornaTodosAtendimentos()
        {
            await using var ctx = DbContextFactory.Criar();
            await DbContextFactory.CriarUsuarioAsync(ctx);
            var mesa1 = await DbContextFactory.CriarMesaAsync(ctx, numero: 1);
            var mesa2 = await DbContextFactory.CriarMesaAsync(ctx, numero: 2);
            var service = new AtendimentoService(ctx);

            await service.CriarPresencialAsync(mesa1.Id);
            await service.CriarPresencialAsync(mesa2.Id);

            var resultado = await service.ListarPresenciaisAsync();

            Assert.Equal(2, resultado.Count());
        }
    }
}
