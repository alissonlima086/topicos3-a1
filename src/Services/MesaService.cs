using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class MesaService : IMesaService
    {
        private readonly ApplicationDbContext _context;

        public MesaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mesa>> ListarTodosAsync()
        {
            return await _context.Mesas.ToListAsync();
        }

        public async Task<Mesa?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Mesas.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<(Mesa? mesa, string? erro)> CriarAsync(Mesa mesa)
        {
            var numeroExiste = await _context.Mesas.AnyAsync(m => m.Numero == mesa.Numero);
            if (numeroExiste)
                return (null, "Já existe uma mesa com esse número.");

            mesa.Id = Guid.NewGuid();
            _context.Add(mesa);
            await _context.SaveChangesAsync();
            return (mesa, null);
        }

        public async Task<Mesa?> EditarAsync(Guid id, Mesa mesa)
        {
            if (!Existe(id)) return null;
            _context.Update(mesa);
            await _context.SaveChangesAsync();
            return mesa;
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa == null) return false;
            _context.Mesas.Remove(mesa);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Existe(Guid id)
        {
            return _context.Mesas.Any(m => m.Id == id);
        }
    }
}
