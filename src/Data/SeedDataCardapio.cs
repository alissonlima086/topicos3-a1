using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Enums;

public static class SeedDataCardapio
{
    public static async Task Popular(
        IServiceProvider serviceProvider,
        Mesa[] mesas,
        List<Usuario> usuarios)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Cardápios de hoje
        if (!await context.Cardapios.AnyAsync())
        {
            var hoje = DateTime.Today;

            // Busca pratos ativos de cada turno (já persistidos)
            var pratosAlmoco = await context.Pratos
                .Where(p => p.Ativo && p.Turno == Turno.Almoco)
                .ToListAsync();

            var pratosJantar = await context.Pratos
                .Where(p => p.Ativo && p.Turno == Turno.Jantar)
                .ToListAsync();

            // Cardápio do Almoço
            if (pratosAlmoco.Any())
            {
                var cardapioAlmoco = new Cardapio { Data = hoje, Turno = Turno.Almoco };
                context.Cardapios.Add(cardapioAlmoco);
                await context.SaveChangesAsync();

                var itensAlmoco = pratosAlmoco.Select((p, i) => new ItemCardapio
                {
                    CardapioId = cardapioAlmoco.Id,
                    PratoId    = p.Id,
                    IsSugestao = i == 0  // primeiro prato vira sugestão do chef
                }).ToList();

                context.ItensCardapio.AddRange(itensAlmoco);
            }

            // Cardápio do Jantar
            if (pratosJantar.Any())
            {
                var cardapioJantar = new Cardapio { Data = hoje, Turno = Turno.Jantar };
                context.Cardapios.Add(cardapioJantar);
                await context.SaveChangesAsync();

                var itensJantar = pratosJantar.Select((p, i) => new ItemCardapio
                {
                    CardapioId = cardapioJantar.Id,
                    PratoId    = p.Id,
                    IsSugestao = i == 0  // primeiro prato vira sugestão do chef
                }).ToList();

                context.ItensCardapio.AddRange(itensJantar);
            }

            await context.SaveChangesAsync();
        }

        // Reservas (somente jantar, 18h+)
        if (!await context.Reservas.AnyAsync() && usuarios.Count >= 3 && mesas.Length >= 4)
        {
            var hoje = DateTime.Today;

            var reservas = new[]
            {
                new Reserva
                {
                    Data           = hoje,
                    HorarioInicio  = hoje.AddHours(19),
                    HorarioFim     = hoje.AddHours(21),
                    NumeroPessoas  = 2,
                    UsuarioId      = usuarios[0].Id,
                    MesaId         = mesas[0].Id,
                    Status         = Status.Confirmado
                },
                new Reserva
                {
                    Data           = hoje,
                    HorarioInicio  = hoje.AddHours(20),
                    HorarioFim     = hoje.AddHours(22),
                    NumeroPessoas  = 4,
                    UsuarioId      = usuarios[1].Id,
                    MesaId         = mesas[2].Id,
                    Status         = Status.Pendente
                },
                new Reserva
                {
                    Data           = hoje.AddDays(1),
                    HorarioInicio  = hoje.AddDays(1).AddHours(19).AddMinutes(30),
                    HorarioFim     = hoje.AddDays(1).AddHours(21).AddMinutes(30),
                    NumeroPessoas  = 3,
                    UsuarioId      = usuarios[2].Id,
                    MesaId         = mesas[3].Id,
                    Status         = Status.Pendente
                },
                new Reserva
                {
                    Data           = hoje.AddDays(-1),
                    HorarioInicio  = hoje.AddDays(-1).AddHours(18).AddMinutes(30),
                    HorarioFim     = hoje.AddDays(-1).AddHours(20),
                    NumeroPessoas  = 2,
                    UsuarioId      = usuarios[0].Id,
                    MesaId         = mesas[1].Id,
                    Status         = Status.Finalizado
                },
            };

            context.Reservas.AddRange(reservas);
            await context.SaveChangesAsync();
        }

        // ── Atendimentos presenciais de exemplo ───────────────────────────────
        if (!await context.Atendimentos.AnyAsync() && mesas.Length >= 2)
        {
            // Busca pratos para montar pedidos dos atendimentos
            var pratoA = await context.Pratos.FirstOrDefaultAsync(p => p.Turno == Turno.Jantar && p.Ativo);
            var pratoB = await context.Pratos.FirstOrDefaultAsync(p => p.Turno == Turno.Almoco && p.Ativo);

            if (pratoA != null)
            {
                // Atendimento 1 — em andamento
                var pedidoAten1 = new Pedido
                {
                    UsuarioId   = usuarios.Count > 0 ? usuarios[0].Id : Guid.Empty,
                    DataHora    = DateTime.Now.AddMinutes(-30),
                    Status      = Status.EmAtendimento,
                    TaxaEntrega = 0f,
                    PrecoTotal  = pratoA.PrecoBase
                };
                context.Pedidos.Add(pedidoAten1);
                await context.SaveChangesAsync();

                context.ItensPedido.Add(new ItemPedido
                {
                    PedidoId      = pedidoAten1.Id,
                    NomePrato     = pratoA.Nome,
                    Quantidade    = 1,
                    PrecoUnitario = pratoA.PrecoBase,
                    FoiSugestao   = false,
                    Observacao    = ""
                });

                var atendimento1 = new AtendimentoPresencial(mesas[0].Id)
                {
                    PedidoId = pedidoAten1.Id,
                    Status   = Status.EmAtendimento
                };
                context.AtendimentosPresenciais.Add(atendimento1);
            }

            if (pratoB != null && mesas.Length >= 2)
            {
                // Atendimento 2 — finalizado
                var pedidoAten2 = new Pedido
                {
                    UsuarioId   = usuarios.Count > 1 ? usuarios[1].Id : Guid.Empty,
                    DataHora    = DateTime.Now.AddHours(-2),
                    Status      = Status.Finalizado,
                    TaxaEntrega = 0f,
                    PrecoTotal  = pratoB.PrecoBase * 2
                };
                context.Pedidos.Add(pedidoAten2);
                await context.SaveChangesAsync();

                context.ItensPedido.Add(new ItemPedido
                {
                    PedidoId      = pedidoAten2.Id,
                    NomePrato     = pratoB.Nome,
                    Quantidade    = 2,
                    PrecoUnitario = pratoB.PrecoBase,
                    FoiSugestao   = true,
                    Observacao    = "Sem cebola"
                });

                var atendimento2 = new AtendimentoPresencial(mesas[1].Id)
                {
                    PedidoId = pedidoAten2.Id,
                    Status   = Status.Finalizado
                };
                context.AtendimentosPresenciais.Add(atendimento2);
            }

            await context.SaveChangesAsync();
        }
    }
}
