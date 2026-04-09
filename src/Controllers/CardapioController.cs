using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Enums;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CardapioController : Controller
    {
        private readonly ICardapioService _cardapioService;

        public CardapioController(ICardapioService cardapioService)
        {
            _cardapioService = cardapioService;
        }

        public async Task<IActionResult> Index()
        {
            var cardapios = await _cardapioService.ListarTodosAsync();
            return View(cardapios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gerar(DateTime data, Turno turno)
        {
            var (cardapio, erro) = await _cardapioService.GerarAsync(data, turno);

            if (erro != null)
                TempData["Erro"] = erro;
            else
                TempData["Sucesso"] = $"Cardápio de {data:dd/MM/yyyy} — {turno} gerado com sucesso!";

            return RedirectToAction(nameof(Index));
        }
    }
}
