using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IEnderecoService
    {
        Task<IEnumerable<Endereco>> ListarTodosAsync();
        Task<Endereco?> BuscarPorIdAsync(Guid id);
        Task<(Endereco? endereco, string? erro)> CriarAsync(Endereco endereco);
        Task<Endereco?> EditarAsync(Guid id, Endereco endereco);
        Task<bool> ExcluirAsync(Guid id);
        Task<Usuario?> BuscarUsuarioPorCpfAsync(string cpf);
        Task<Usuario?> BuscarUsuarioPorIdAsync(Guid id);
        bool Existe(Guid id);
    }
}