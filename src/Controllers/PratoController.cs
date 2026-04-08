using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class PratoController : Controller
    {
        private readonly IPratoService _pratoService;

        public PratoController(IPratoService pratoService)
        {
            _pratoService = pratoService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _pratoService.ListarTodosAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var prato = await _pratoService.BuscarPorIdAsync(id.Value);
            if (prato == null) return NotFound();
            return View(prato);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,PrecoBase,Ativo")] Prato prato)
        {
            if (ModelState.IsValid)
            {
                var (resultado, erro) = await _pratoService.CriarAsync(prato);
                if (erro != null)
                {
                    ModelState.AddModelError("Nome", erro);
                    return View(prato);
                }
                // Redireciona para Edit para poder adicionar ingredientes
                return RedirectToAction(nameof(Edit), new { id = resultado!.Id });
            }
            return View(prato);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var vm = await _pratoService.BuscarParaEdicaoAsync(id.Value);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, PratoEditViewModel vm, List<Guid> ingredientesSelecionados)
        {
            if (id != vm.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var prato = new Prato
                {
                    Id = vm.Id,
                    Nome = vm.Nome,
                    Descricao = vm.Descricao,
                    PrecoBase = vm.PrecoBase,
                    Ativo = vm.Ativo
                };

                try
                {
                    var resultado = await _pratoService.EditarAsync(id, prato, ingredientesSelecionados);
                    if (resultado == null) return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_pratoService.Existe(vm.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Re-carrega ingredientes se houver erro de validação
            var vmReload = await _pratoService.BuscarParaEdicaoAsync(id);
            if (vmReload == null) return NotFound();
            vmReload.Nome = vm.Nome;
            vmReload.Descricao = vm.Descricao;
            vmReload.PrecoBase = vm.PrecoBase;
            vmReload.Ativo = vm.Ativo;
            return View(vmReload);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var prato = await _pratoService.BuscarPorIdAsync(id.Value);
            if (prato == null) return NotFound();
            return View(prato);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _pratoService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleAtivo(Guid id)
        {
            var resultado = await _pratoService.ToggleAtivoAsync(id);
            if (resultado == null) return NotFound();
            return RedirectToAction(nameof(Index));
        }
    }
}
