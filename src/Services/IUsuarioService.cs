using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> ListarTodosAsync();
        Task<Usuario?> BuscarPorIdAsync(Guid id);
        Task<Usuario> CriarAsync(Usuario usuario);
        Task<Usuario?> EditarAsync(Guid id, Usuario usuario);
        Task<bool> ExcluirAsync(Guid id);
        Task<bool> AlterarSenhaAsync(Guid id, string senhaAtual, string novaSenha);
        bool Existe(Guid id);
    }
}