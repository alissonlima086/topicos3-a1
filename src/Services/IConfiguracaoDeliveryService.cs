using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IConfiguracaoDeliveryService
    {
        Task<IEnumerable<ConfiguracaoDelivery>> ListarTodosAsync();
        Task<ConfiguracaoDelivery?> BuscarPorIdAsync(Guid id);
        Task<(ConfiguracaoDelivery? config, string? erro)> CriarAsync(ConfiguracaoDelivery config);
        Task<ConfiguracaoDelivery?> EditarAsync(Guid id, ConfiguracaoDelivery config);
        Task<bool> ExcluirAsync(Guid id);
        Task<bool> ExisteDeliveryProprioAsync();
    }
}
