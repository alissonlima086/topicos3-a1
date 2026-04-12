using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAtendimentoService
    {
        Task<IEnumerable<AtendimentoPresencial>> ListarPresenciaisAsync();
        Task<AtendimentoPresencial?> BuscarPresencialPorIdAsync(Guid id);
        Task<bool> FinalizarAsync(Guid id);
        Task<(AtendimentoPresencial? atendimento, string? erro)> CriarPresencialAsync(Guid mesaId);
        Task<(bool ok, string? erro)> AdicionarItensDoCardapioAsync(Guid atendimentoId, List<ItemCardapioSelecionado> selecionados);
    }

    public class ItemCardapioSelecionado
    {
        public Guid ItemCardapioId { get; set; }
        public int Quantidade { get; set; }
        public string? Observacao { get; set; }
    }
}
