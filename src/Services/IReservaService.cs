using WebApplication1.Models;
using WebApplication1.Models.Enums;

namespace WebApplication1.Services
{
    public interface IReservaService
    {
        Task<IEnumerable<Reserva>> ListarTodosAsync();
        Task<IEnumerable<Reserva>> ListarPorUsuarioAsync(Guid usuarioId);
        Task<Reserva?> BuscarPorIdAsync(Guid id);
        Task<(Reserva? reserva, string? erro)> CriarAsync(Reserva reserva);
        Task<Reserva?> EditarAsync(Guid id, Reserva reserva);
        Task<bool> ExcluirAsync(Guid id);
        Task<Reserva?> AtualizarStatusAsync(Guid id, Status status);
        Task<Usuario?> BuscarUsuarioPorCpfAsync(string cpf);
        Task<Usuario?> BuscarUsuarioPorIdAsync(Guid id);
        bool Existe(Guid id);
    }
}