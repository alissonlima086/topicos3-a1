using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.ViewModel;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RelatorioController : Controller
    {
        private readonly IRelatorioService _service;

        public RelatorioController(IRelatorioService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(PeriodoRelatorio periodo = PeriodoRelatorio.Mes)
        {
            var vm = await _service.GerarAsync(periodo);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Filtrar(PeriodoRelatorio periodo,
            DateTime? dataInicio, DateTime? dataFim)
        {
            var vm = await _service.GerarAsync(periodo, dataInicio, dataFim);
            return View("Index", vm);
        }
    }
}
