using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IMesaService
    {
        Task<IEnumerable<Mesa>> ListarTodosAsync();
        Task<Mesa?> BuscarPorIdAsync(Guid id);
        Task<(Mesa? mesa, string? erro)> CriarAsync(Mesa mesa);
        Task<Mesa?> EditarAsync(Guid id, Mesa mesa);
        Task<bool> ExcluirAsync(Guid id);
        bool Existe(Guid id);
    }
}
