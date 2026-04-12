using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Enums;

namespace WebApplication1.Services
{
    public class CardapioService : ICardapioService
    {
        private readonly ApplicationDbContext _context;
        private static readonly Random _rng = new();

        public CardapioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cardapio>> ListarTodosAsync()
        {
            return await _context.Cardapios
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Prato)
                .OrderByDescending(c => c.Data)
                .ThenBy(c => c.Turno)
                .ToListAsync();
        }

        public async Task<Cardapio?> BuscarAtualAsync(DateTime data, Turno turno)
        {
            return await _context.Cardapios
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Prato)
                .FirstOrDefaultAsync(c => c.Data.Date == data.Date && c.Turno == turno);
        }

        public async Task<(Cardapio? cardapio, string? erro)> GerarAsync(DateTime data, Turno turno)
        {
            var pratosAtivos = await _context.Pratos
                .Where(p => p.Ativo)
                .ToListAsync();

            if (pratosAtivos.Count == 0)
                return (null, "Nenhum prato ativo cadastrado para gerar o cardápio.");

            var anterior = await _context.Cardapios
                .FirstOrDefaultAsync(c => c.Data.Date == data.Date && c.Turno == turno);

            if (anterior != null)
                _context.Cardapios.Remove(anterior);

            var sorteados = pratosAtivos
                .OrderBy(_ => _rng.Next())
                .Take(Math.Min(20, pratosAtivos.Count))
                .ToList();

            var sugestaoIdx = _rng.Next(sorteados.Count);

            var cardapio = new Cardapio
            {
                Data = data.Date,
                Turno = turno,
                Itens = sorteados.Select((p, idx) => new ItemCardapio
                {
                    PratoId = p.Id,
                    IsSugestao = idx == sugestaoIdx
                }).ToList()
            };

            _context.Cardapios.Add(cardapio);
            await _context.SaveChangesAsync();

            return (await BuscarAtualAsync(data, turno), null);
        }
    }
}
