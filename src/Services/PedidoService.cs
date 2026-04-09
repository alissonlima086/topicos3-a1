using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Enums;

namespace WebApplication1.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly ApplicationDbContext _context;

        public PedidoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pedido>> ListarTodosAsync()
        {
            return await _context.Pedidos.ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> ListarPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Pedidos.Where(p => p.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<Pedido?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(Pedido? pedido, string? erro)> CriarAsync(Pedido pedido)
        {
            var usuarioExiste = await _context.Users.AnyAsync(u => u.Id == pedido.UsuarioId);
            if (!usuarioExiste)
                return (null, "Usuário não encontrado.");

            pedido.Id = Guid.NewGuid();
            pedido.DataHora = DateTime.Now;
            pedido.Status = Status.Pendente;
            _context.Add(pedido);
            await _context.SaveChangesAsync();
            return (pedido, null);
        }

        public async Task<Pedido?> EditarAsync(Guid id, Pedido pedido)
        {
            if (!Existe(id)) return null;
            _context.Update(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return false;
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Pedido?> AtualizarStatusAsync(Guid id, Status status)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return null;
            pedido.AtualizarStatus(status);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public bool Existe(Guid id)
        {
            return _context.Pedidos.Any(p => p.Id == id);
        }
    }
}
