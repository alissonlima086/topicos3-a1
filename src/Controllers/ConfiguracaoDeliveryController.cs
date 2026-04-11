using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfiguracaoDeliveryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConfiguracaoDeliveryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var configs = await _context.ConfiguracoesDelivery.ToListAsync();
            return View(configs);
        }

        public async Task<IActionResult> Create()
        {
            bool jaTemProprio = await _context.ConfiguracoesDelivery
                .AnyAsync(c => c.Tipo == TipoDelivery.Proprio);
            ViewBag.JaTemProprio = jaTemProprio;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConfiguracaoDelivery model)
        {
            if (model.Tipo == TipoDelivery.Proprio)
            {
                bool jaTemProprio = await _context.ConfiguracoesDelivery
                    .AnyAsync(c => c.Tipo == TipoDelivery.Proprio);
                if (jaTemProprio)
                {
                    ModelState.AddModelError("", "Já existe uma configuração de delivery próprio. Edite a existente.");
                    ViewBag.JaTemProprio = true;
                    return View(model);
                }
            }

            if (!ModelState.IsValid) return View(model);

            _context.ConfiguracoesDelivery.Add(model);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Configuração cadastrada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var config = await _context.ConfiguracoesDelivery.FindAsync(id);
            if (config == null) return NotFound();
            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ConfiguracaoDelivery model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _context.Update(model);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Configuração atualizada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var config = await _context.ConfiguracoesDelivery.FindAsync(id);
            if (config != null)
            {
                _context.ConfiguracoesDelivery.Remove(config);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AtendimentosPresenciais()
        {
            var atendimentos = await _context.AtendimentosPresenciais
                .Include(a => a.Mesa)
                .Include(a => a.Pedido)
                .ToListAsync();
            return View(atendimentos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarAtendimento(Guid id)
        {
            var atendimento = await _context.AtendimentosPresenciais.FindAsync(id);
            if (atendimento != null)
            {
                atendimento.Finalizar();
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AtendimentosPresenciais));
        }
    }
}
