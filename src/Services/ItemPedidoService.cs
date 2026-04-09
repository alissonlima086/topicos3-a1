using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ItemPedidoService : IItemPedidoService
    {
        private readonly ApplicationDbContext _context;

        public ItemPedidoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemPedido>> ListarPorPedidoAsync(Guid pedidoId)
        {
            return await _context.ItensPedido.Where(i => i.PedidoId == pedidoId).ToListAsync();
        }

        public async Task<ItemPedido?> BuscarPorIdAsync(Guid id)
        {
            return await _context.ItensPedido.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<(ItemPedido? item, string? erro)> AdicionarAsync(ItemPedido item)
        {
            var pedidoExiste = await _context.Pedidos.AnyAsync(p => p.Id == item.PedidoId);
            if (!pedidoExiste)
                return (null, "Pedido não encontrado.");

            if (item.Quantidade <= 0)
                return (null, "Quantidade deve ser maior que zero.");

            item.Id = Guid.NewGuid();
            _context.Add(item);

            // recalcular preco pedito
            var pedido = await _context.Pedidos.FindAsync(item.PedidoId);
            if (pedido != null)
            {
                pedido.PrecoTotal += item.PrecoUnitario * item.Quantidade;
            }

            await _context.SaveChangesAsync();
            return (item, null);
        }

        public async Task<ItemPedido?> EditarAsync(Guid id, ItemPedido item)
        {
            if (!Existe(id)) return null;

            var anterior = await _context.ItensPedido.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            _context.Update(item);

            // Ajusta o preço total do pedido
            if (anterior != null)
            {
                var pedido = await _context.Pedidos.FindAsync(item.PedidoId);
                if (pedido != null)
                {
                    pedido.PrecoTotal -= anterior.PrecoUnitario * anterior.Quantidade;
                    pedido.PrecoTotal += item.PrecoUnitario * item.Quantidade;
                }
            }

            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> RemoverAsync(Guid id)
        {
            var item = await _context.ItensPedido.FindAsync(id);
            if (item == null) return false;

            var pedido = await _context.Pedidos.FindAsync(item.PedidoId);
            if (pedido != null)
            {
                pedido.PrecoTotal -= item.PrecoUnitario * item.Quantidade;
            }

            _context.ItensPedido.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Existe(Guid id)
        {
            return _context.ItensPedido.Any(i => i.Id == id);
        }
    }
}
