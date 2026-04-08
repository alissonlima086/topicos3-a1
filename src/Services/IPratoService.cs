using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Services
{
    public interface IPratoService
    {
        Task<IEnumerable<Prato>> ListarTodosAsync();
        Task<IEnumerable<Prato>> ListarAtivosAsync();
        Task<Prato?> BuscarPorIdAsync(Guid id);
        Task<PratoEditViewModel?> BuscarParaEdicaoAsync(Guid id);
        Task<(Prato? prato, string? erro)> CriarAsync(Prato prato);
        Task<Prato?> EditarAsync(Guid id, Prato prato, List<Guid> ingredientesSelecionados);
        Task<bool> ExcluirAsync(Guid id);
        Task<Prato?> ToggleAtivoAsync(Guid id);
        bool Existe(Guid id);
    }
}
