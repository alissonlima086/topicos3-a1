using WebApplication1.Models.ViewModel;

namespace WebApplication1.Services
{
    public interface IRelatorioService
    {
        Task<RelatorioViewModel> GerarAsync(PeriodoRelatorio periodo,
            DateTime? dataInicio = null, DateTime? dataFim = null);
    }
}
