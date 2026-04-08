using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _pedidoService.ListarTodosAsync());
        }

        public async Task<IActionResult> MeusPedidos(Guid usuarioId)
        {
            return View(await _pedidoService.ListarPorUsuarioAsync(usuarioId));
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var pedido = await _pedidoService.BuscarPorIdAsync(id.Value);
            if (pedido == null) return NotFound();
            return View(pedido);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsuarioId,TaxaEntrega")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                var (resultado, erro) = await _pedidoService.CriarAsync(pedido);
                if (erro != null)
                {
                    ModelState.AddModelError(string.Empty, erro);
                    return View(pedido);
                }
                return RedirectToAction(nameof(Details), new { id = resultado!.Id });
            }
            return View(pedido);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var pedido = await _pedidoService.BuscarPorIdAsync(id.Value);
            if (pedido == null) return NotFound();
            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,DataHora,PrecoTotal,TaxaEntrega,Status,UsuarioId")] Pedido pedido)
        {
            if (id != pedido.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var resultado = await _pedidoService.EditarAsync(id, pedido);
                    if (resultado == null) return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_pedidoService.Existe(pedido.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var pedido = await _pedidoService.BuscarPorIdAsync(id.Value);
            if (pedido == null) return NotFound();
            return View(pedido);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _pedidoService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AtualizarStatus(Guid id, Status status)
        {
            var resultado = await _pedidoService.AtualizarStatusAsync(id, status);
            if (resultado == null) return NotFound();
            return RedirectToAction(nameof(Index));
        }
    }
}
