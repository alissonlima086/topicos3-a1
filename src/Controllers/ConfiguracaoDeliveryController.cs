using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfiguracaoDeliveryController : Controller
    {
        private readonly IConfiguracaoDeliveryService _service;

        public ConfiguracaoDeliveryController(IConfiguracaoDeliveryService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.ListarTodosAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.JaTemProprio = await _service.ExisteDeliveryProprioAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConfiguracaoDelivery model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.JaTemProprio = await _service.ExisteDeliveryProprioAsync();
                return View(model);
            }

            var (config, erro) = await _service.CriarAsync(model);
            if (erro != null)
            {
                ModelState.AddModelError("", erro);
                ViewBag.JaTemProprio = true;
                return View(model);
            }

            TempData["Sucesso"] = "Configuração cadastrada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var config = await _service.BuscarPorIdAsync(id);
            if (config == null) return NotFound();
            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ConfiguracaoDelivery model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var result = await _service.EditarAsync(id, model);
            if (result == null) return NotFound();

            TempData["Sucesso"] = "Configuração atualizada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
