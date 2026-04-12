using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Services
{
    public class PratoService : IPratoService
    {
        private readonly ApplicationDbContext _context;

        public PratoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prato>> ListarTodosAsync()
        {
            return await _context.Pratos
                .Include(p => p.PratoIngredientes)
                    .ThenInclude(pi => pi.Ingrediente)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prato>> ListarAtivosAsync()
        {
            return await _context.Pratos
                .Where(p => p.Ativo)
                .Include(p => p.PratoIngredientes)
                    .ThenInclude(pi => pi.Ingrediente)
                .ToListAsync();
        }

        public async Task<Prato?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Pratos
                .Include(p => p.PratoIngredientes)
                    .ThenInclude(pi => pi.Ingrediente)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PratoEditViewModel?> BuscarParaEdicaoAsync(Guid id)
        {
            var prato = await _context.Pratos
                .Include(p => p.PratoIngredientes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prato == null) return null;

            var todosIngredientes = await _context.Ingredientes.ToListAsync();
            var selecionados = prato.PratoIngredientes.Select(pi => pi.IngredienteId).ToHashSet();

            return new PratoEditViewModel
            {
                Id = prato.Id,
                Nome = prato.Nome,
                Descricao = prato.Descricao,
                PrecoBase = prato.PrecoBase,
                Ativo = prato.Ativo,
                Turno = prato.Turno,
                Ingredientes = todosIngredientes.Select(i => new IngredienteSelecaoViewModel
                {
                    Id = i.Id,
                    Nome = i.Nome,
                    Descricao = i.Descricao,
                    Selecionado = selecionados.Contains(i.Id)
                }).ToList()
            };
        }

        public async Task<(Prato? prato, string? erro)> CriarAsync(Prato prato)
        {
            var nomeExiste = await _context.Pratos.AnyAsync(p => p.Nome == prato.Nome);
            if (nomeExiste)
                return (null, "Já existe um prato com esse nome.");

            prato.Id = Guid.NewGuid();
            _context.Add(prato);
            await _context.SaveChangesAsync();
            return (prato, null);
        }

        public async Task<Prato?> EditarAsync(Guid id, Prato prato, List<Guid> ingredientesSelecionados)
        {
            var existente = await _context.Pratos.FindAsync(id);
            if (existente == null) return null;

            existente.Nome = prato.Nome;
            existente.Descricao = prato.Descricao;
            existente.PrecoBase = prato.PrecoBase;
            existente.Ativo = prato.Ativo;
            existente.Turno = prato.Turno;

            // Remove ingredientes antigos e insere os novos
            var antigas = _context.PratoIngredientes.Where(pi => pi.PratoId == id);
            _context.PratoIngredientes.RemoveRange(antigas);

            foreach (var ingId in ingredientesSelecionados)
            {
                _context.PratoIngredientes.Add(new PratoIngrediente
                {
                    PratoId = id,
                    IngredienteId = ingId
                });
            }

            await _context.SaveChangesAsync();
            return existente;
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var prato = await _context.Pratos.FindAsync(id);
            if (prato == null) return false;
            _context.Pratos.Remove(prato);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Prato?> ToggleAtivoAsync(Guid id)
        {
            var prato = await _context.Pratos.FindAsync(id);
            if (prato == null) return null;
            if (prato.Ativo) prato.Desativar(); else prato.Ativar();
            await _context.SaveChangesAsync();
            return prato;
        }

        public bool Existe(Guid id)
        {
            return _context.Pratos.Any(p => p.Id == id);
        }
    }
}
