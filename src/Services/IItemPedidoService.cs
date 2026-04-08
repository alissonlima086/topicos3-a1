using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IItemPedidoService
    {
        Task<IEnumerable<ItemPedido>> ListarPorPedidoAsync(Guid pedidoId);
        Task<ItemPedido?> BuscarPorIdAsync(Guid id);
        Task<(ItemPedido? item, string? erro)> AdicionarAsync(ItemPedido item);
        Task<ItemPedido?> EditarAsync(Guid id, ItemPedido item);
        Task<bool> RemoverAsync(Guid id);
        bool Existe(Guid id);
    }
}
