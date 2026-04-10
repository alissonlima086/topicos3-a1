using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class MesaController : Controller
    {
        private readonly IMesaService _mesaService;

        public MesaController(IMesaService mesaService)
        {
            _mesaService = mesaService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _mesaService.ListarTodosAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var mesa = await _mesaService.BuscarPorIdAsync(id.Value);
            if (mesa == null) return NotFound();
            return View(mesa);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Numero,Capacidade,Disponivel")] Mesa mesa)
        {
            if (ModelState.IsValid)
            {
                var (resultado, erro) = await _mesaService.CriarAsync(mesa);
                if (erro != null)
                {
                    ModelState.AddModelError("Numero", erro);
                    return View(mesa);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mesa);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var mesa = await _mesaService.BuscarPorIdAsync(id.Value);
            if (mesa == null) return NotFound();
            return View(mesa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Numero,Capacidade,Disponivel")] Mesa mesa)
        {
            if (id != mesa.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var resultado = await _mesaService.EditarAsync(id, mesa);
                    if (resultado == null) return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_mesaService.Existe(mesa.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mesa);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var mesa = await _mesaService.BuscarPorIdAsync(id.Value);
            if (mesa == null) return NotFound();
            return View(mesa);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _mesaService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
