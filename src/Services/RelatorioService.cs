using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Models.ViewModel;

namespace WebApplication1.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly ApplicationDbContext _context;

        public RelatorioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RelatorioViewModel> GerarAsync(PeriodoRelatorio periodo,
            DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var (inicio, fim) = dataInicio.HasValue && dataFim.HasValue
                ? (dataInicio.Value.Date, dataFim.Value.Date.AddDays(1).AddTicks(-1))
                : CalcularPeriodo(periodo);

            var pedidos = await _context.Pedidos
                .Include(p => p.Atendimento)
                .Include(p => p.Itens)
                .Where(p => p.DataHora >= inicio
                         && p.DataHora <= fim
                         && p.Itens.Any())
                .ToListAsync();

            var presenciais = pedidos
                .Where(p => p.Atendimento is AtendimentoPresencial { Status: Status.Finalizado })
                .ToList();

            var delivery = pedidos
                .Where(p => p.Atendimento is not AtendimentoPresencial)
                .Where(p => p.Status != Status.Cancelado)
                .ToList();

            var todosPedidosValidos = presenciais.Concat(delivery).ToList();

            var faturamento = new List<FaturamentoTipoDto>();

            if (presenciais.Any())
                faturamento.Add(new FaturamentoTipoDto
                {
                    TipoAtendimento = "Presencial (Mesa)",
                    Total = presenciais.Sum(p => p.PrecoTotal),
                    Quantidade = presenciais.Count
                });

            var porApp = delivery
                .Where(p => p.Atendimento is AtendimentoDeliveryApp)
                .GroupBy(p => ((AtendimentoDeliveryApp)p.Atendimento!).NomeApp);

            foreach (var g in porApp)
                faturamento.Add(new FaturamentoTipoDto
                {
                    TipoAtendimento = g.Key,
                    Total = g.Sum(p => p.PrecoTotal),
                    Quantidade = g.Count()
                });

            var deliveryProprio = delivery
                .Where(p => p.Atendimento is AtendimentoDeliveryProprio)
                .ToList();
            if (deliveryProprio.Any())
                faturamento.Add(new FaturamentoTipoDto
                {
                    TipoAtendimento = "Delivery Próprio",
                    Total = deliveryProprio.Sum(p => p.PrecoTotal),
                    Quantidade = deliveryProprio.Count
                });

            var semAtendimento = delivery.Where(p => p.Atendimento is null).ToList();
            if (semAtendimento.Any())
                faturamento.Add(new FaturamentoTipoDto
                {
                    TipoAtendimento = "Delivery (pedido online)",
                    Total = semAtendimento.Sum(p => p.PrecoTotal),
                    Quantidade = semAtendimento.Count
                });

            faturamento = faturamento.OrderByDescending(f => f.Total).ToList();

            var todosItens = todosPedidosValidos.SelectMany(p => p.Itens).ToList();

            var itensMaisVendidos = todosItens
                .Where(i => !i.FoiSugestao)
                .GroupBy(i => i.NomePrato)
                .Select(g => new ItemMaisVendidoDto
                {
                    NomePrato = g.Key,
                    FoiSugestao = false,
                    TotalVendido = g.Sum(i => i.Quantidade),
                    ReceitaTotal = g.Sum(i => i.Quantidade * i.PrecoUnitario)
                })
                .OrderByDescending(i => i.TotalVendido)
                .Take(10)
                .ToList();

            var sugestoesMaisVendidas = todosItens
                .Where(i => i.FoiSugestao)
                .GroupBy(i => i.NomePrato)
                .Select(g => new ItemMaisVendidoDto
                {
                    NomePrato = g.Key,
                    FoiSugestao = true,
                    TotalVendido = g.Sum(i => i.Quantidade),
                    ReceitaTotal = g.Sum(i => i.Quantidade * i.PrecoUnitario)
                })
                .OrderByDescending(i => i.TotalVendido)
                .Take(10)
                .ToList();

            return new RelatorioViewModel
            {
                Periodo = periodo,
                DataInicio = inicio,
                DataFim = fim,
                FaturamentoPorTipo = faturamento,
                FaturamentoTotal = todosPedidosValidos.Sum(p => p.PrecoTotal),
                TotalPedidos = todosPedidosValidos.Count,
                ItensMaisVendidos = itensMaisVendidos,
                SugestoesMaisVendidas = sugestoesMaisVendidas
            };
        }

        private static (DateTime inicio, DateTime fim) CalcularPeriodo(PeriodoRelatorio periodo)
        {
            var hoje = DateTime.Today;
            return periodo switch
            {
                PeriodoRelatorio.Semana => (
                    hoje.AddDays(-(int)hoje.DayOfWeek),
                    hoje.AddDays(7 - (int)hoje.DayOfWeek).AddTicks(-1)
                ),
                PeriodoRelatorio.Ano => (
                    new DateTime(hoje.Year, 1, 1),
                    new DateTime(hoje.Year, 12, 31, 23, 59, 59)
                ),
                _ => (
                    new DateTime(hoje.Year, hoje.Month, 1),
                    new DateTime(hoje.Year, hoje.Month, 1).AddMonths(1).AddTicks(-1)
                )
            };
        }
    }
}
