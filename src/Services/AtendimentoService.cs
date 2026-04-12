using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Enums;

namespace WebApplication1.Services
{
    public class AtendimentoService : IAtendimentoService
    {
        private readonly ApplicationDbContext _context;

        public AtendimentoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AtendimentoPresencial>> ListarPresenciaisAsync()
        {
            return await _context.AtendimentosPresenciais
                .Include(a => a.Mesa)
                .Include(a => a.Pedido).ThenInclude(p => p!.Itens)
                .OrderByDescending(a => a.Status)
                .ToListAsync();
        }

        public async Task<AtendimentoPresencial?> BuscarPresencialPorIdAsync(Guid id)
        {
            return await _context.AtendimentosPresenciais
                .Include(a => a.Mesa)
                .Include(a => a.Pedido).ThenInclude(p => p!.Itens)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> FinalizarAsync(Guid id)
        {
            var atendimento = await _context.AtendimentosPresenciais
                .Include(a => a.Mesa)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (atendimento == null) return false;
            atendimento.Finalizar();
            if (atendimento.Mesa != null) atendimento.Mesa.Liberar();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(AtendimentoPresencial? atendimento, string? erro)> CriarPresencialAsync(Guid mesaId)
        {
            var mesa = await _context.Mesas.FindAsync(mesaId);
            if (mesa == null) return (null, "Mesa não encontrada.");
            if (!mesa.Disponivel) return (null, $"Mesa {mesa.Numero} já está ocupada.");

            var adminUser = await _context.Users.FirstOrDefaultAsync();
            if (adminUser == null) return (null, "Nenhum usuário cadastrado no sistema.");

            var pedido = new Pedido
            {
                UsuarioId = adminUser.Id,
                DataHora = DateTime.Now,
                Status = Status.EmAtendimento,
                TaxaEntrega = 0,
                PrecoTotal = 0
            };
            _context.Pedidos.Add(pedido);

            var atendimento = new AtendimentoPresencial(mesaId)
            {
                PedidoId = pedido.Id,
                Status = Status.EmAtendimento
            };
            mesa.Ocupar();
            _context.AtendimentosPresenciais.Add(atendimento);
            await _context.SaveChangesAsync();
            return (atendimento, null);
        }

        public async Task<(bool ok, string? erro)> AdicionarItensDoCardapioAsync(
            Guid atendimentoId, List<ItemCardapioSelecionado> selecionados)
        {
            var atendimento = await _context.AtendimentosPresenciais
                .Include(a => a.Pedido).ThenInclude(p => p!.Itens)
                .FirstOrDefaultAsync(a => a.Id == atendimentoId);

            if (atendimento == null) return (false, "Atendimento não encontrado.");
            if (atendimento.Status == Status.Finalizado) return (false, "Atendimento já finalizado.");
            if (atendimento.Pedido == null) return (false, "Pedido não encontrado.");

            var validos = selecionados.Where(s => s.Quantidade > 0).ToList();
            if (!validos.Any()) return (false, "Nenhum item com quantidade informada.");

            // Carrega os ItemCardapio referenciando os pratos para calcular preço e nome
            var ids = validos.Select(s => s.ItemCardapioId).ToList();
            var itensCardapio = await _context.ItensCardapio
                .Include(ic => ic.Prato)
                .Where(ic => ids.Contains(ic.Id))
                .ToListAsync();

            foreach (var sel in validos)
            {
                var ic = itensCardapio.FirstOrDefault(x => x.Id == sel.ItemCardapioId);
                if (ic?.Prato == null) continue;

                var preco = ic.IsSugestao ? ic.Prato.PrecoBase * 0.8f : ic.Prato.PrecoBase;
                _context.ItensPedido.Add(new ItemPedido
                {
                    PedidoId = atendimento.PedidoId,
                    NomePrato = ic.Prato.Nome,
                    PrecoUnitario = preco,
                    Quantidade = sel.Quantidade,
                    FoiSugestao = ic.IsSugestao,
                    Observacao = sel.Observacao ?? string.Empty
                });
            }

            await _context.SaveChangesAsync();

            var todosItens = await _context.ItensPedido
                .Where(i => i.PedidoId == atendimento.PedidoId)
                .ToListAsync();
            atendimento.Pedido.PrecoTotal = todosItens.Sum(i => i.PrecoUnitario * i.Quantidade);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}
