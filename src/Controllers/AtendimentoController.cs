using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AtendimentoController : Controller
    {
        private readonly IAtendimentoService _service;
        private readonly IMesaService _mesaService;
        private readonly IReservaService _reservaService;
        private readonly ICardapioService _cardapioService;

        public AtendimentoController(IAtendimentoService service, IMesaService mesaService,
            IReservaService reservaService, ICardapioService cardapioService)
        {
            _service = service;
            _mesaService = mesaService;
            _reservaService = reservaService;
            _cardapioService = cardapioService;
        }

        public async Task<IActionResult> Index()
        {
            var atendimentos = await _service.ListarPresenciaisAsync();
            ViewBag.Reservas = await _reservaService.ListarTodosAsync();
            return View(atendimentos);
        }

        public async Task<IActionResult> Create()
        {
            var mesas = (await _mesaService.ListarTodosAsync()).Where(m => m.Disponivel).ToList();
            ViewBag.Mesas = new SelectList(mesas, "Id", "Numero");
            ViewBag.TemMesa = mesas.Any();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid mesaId)
        {
            var (atendimento, erro) = await _service.CriarPresencialAsync(mesaId);
            if (erro != null)
            {
                TempData["Erro"] = erro;
                return RedirectToAction(nameof(Create));
            }
            TempData["Sucesso"] = "Atendimento aberto. Adicione itens quando quiser.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AdicionarItens(Guid id)
        {
            var atendimento = await _service.BuscarPresencialPorIdAsync(id);
            if (atendimento == null) return NotFound();

            var cardapio = await _cardapioService.BuscarAtualAsync(DateTime.Today, TurnoAtual());
            ViewBag.Cardapio = cardapio;
            ViewBag.TurnoAtual = TurnoAtual().ToString();
            return View(atendimento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarItens(Guid id, List<ItemCardapioSelecionado> itens)
        {
            var (ok, erro) = await _service.AdicionarItensDoCardapioAsync(id, itens);
            if (!ok) TempData["Erro"] = erro;
            else TempData["Sucesso"] = "Itens adicionados.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(Guid id)
        {
            await _service.FinalizarAsync(id);
            TempData["Sucesso"] = "Atendimento finalizado.";
            return RedirectToAction(nameof(Index));
        }

        private static Turno TurnoAtual()
        {
            var h = DateTime.Now.Hour;
            return h >= 13 && h < 18 ? Turno.Almoco : Turno.Jantar;
        }
    }
}
