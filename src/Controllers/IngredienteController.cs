using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class IngredienteController : Controller
    {
        private readonly IIngredienteService _ingredienteService;

        public IngredienteController(IIngredienteService ingredienteService)
        {
            _ingredienteService = ingredienteService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _ingredienteService.ListarTodosAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var ingrediente = await _ingredienteService.BuscarPorIdAsync(id.Value);
            if (ingrediente == null) return NotFound();
            return View(ingrediente);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao")] Ingrediente ingrediente)
        {
            if (ModelState.IsValid)
            {
                var (resultado, erro) = await _ingredienteService.CriarAsync(ingrediente);
                if (erro != null)
                {
                    ModelState.AddModelError("Nome", erro);
                    return View(ingrediente);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ingrediente);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var ingrediente = await _ingredienteService.BuscarPorIdAsync(id.Value);
            if (ingrediente == null) return NotFound();
            return View(ingrediente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Descricao")] Ingrediente ingrediente)
        {
            if (id != ingrediente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var resultado = await _ingredienteService.EditarAsync(id, ingrediente);
                    if (resultado == null) return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_ingredienteService.Existe(ingrediente.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ingrediente);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var ingrediente = await _ingredienteService.BuscarPorIdAsync(id.Value);
            if (ingrediente == null) return NotFound();
            return View(ingrediente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _ingredienteService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
