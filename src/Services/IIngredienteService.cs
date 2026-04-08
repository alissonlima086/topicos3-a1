using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IIngredienteService
    {
        Task<IEnumerable<Ingrediente>> ListarTodosAsync();
        Task<Ingrediente?> BuscarPorIdAsync(Guid id);
        Task<(Ingrediente? ingrediente, string? erro)> CriarAsync(Ingrediente ingrediente);
        Task<Ingrediente?> EditarAsync(Guid id, Ingrediente ingrediente);
        Task<bool> ExcluirAsync(Guid id);
        bool Existe(Guid id);
    }
}
