using WebApplication1.Models;
using WebApplication1.Models.Enums;

namespace WebApplication1.Services
{
    public interface IPedidoService
    {
        Task<IEnumerable<Pedido>> ListarTodosAsync();
        Task<IEnumerable<Pedido>> ListarPorUsuarioAsync(Guid usuarioId);
        Task<Pedido?> BuscarPorIdAsync(Guid id);
        Task<(Pedido? pedido, string? erro)> CriarAsync(Pedido pedido);
        Task<Pedido?> EditarAsync(Guid id, Pedido pedido);
        Task<bool> ExcluirAsync(Guid id);
        Task<Pedido?> AtualizarStatusAsync(Guid id, Status status);
        bool Existe(Guid id);
    }
}
