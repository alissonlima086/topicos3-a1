using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class IngredienteService : IIngredienteService
    {
        private readonly ApplicationDbContext _context;

        public IngredienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ingrediente>> ListarTodosAsync()
        {
            return await _context.Ingredientes.ToListAsync();
        }

        public async Task<Ingrediente?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Ingredientes.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<(Ingrediente? ingrediente, string? erro)> CriarAsync(Ingrediente ingrediente)
        {
            var nomeExiste = await _context.Ingredientes.AnyAsync(i => i.Nome == ingrediente.Nome);
            if (nomeExiste)
                return (null, "Já existe um ingrediente com esse nome.");

            ingrediente.Id = Guid.NewGuid();
            _context.Add(ingrediente);
            await _context.SaveChangesAsync();
            return (ingrediente, null);
        }

        public async Task<Ingrediente?> EditarAsync(Guid id, Ingrediente ingrediente)
        {
            if (!Existe(id)) return null;
            _context.Update(ingrediente);
            await _context.SaveChangesAsync();
            return ingrediente;
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null) return false;
            _context.Ingredientes.Remove(ingrediente);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Existe(Guid id)
        {
            return _context.Ingredientes.Any(i => i.Id == id);
        }
    }
}
