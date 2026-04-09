using WebApplication1.Models;
using WebApplication1.Models.Enums;

namespace WebApplication1.Services
{
    public interface ICardapioService
    {
        Task<Cardapio?> BuscarAtualAsync(DateTime data, Turno turno);
        Task<IEnumerable<Cardapio>> ListarTodosAsync();
        Task<(Cardapio? cardapio, string? erro)> GerarAsync(DateTime data, Turno turno);
    }
}
