using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class EnderecoController : Controller
    {
        private readonly EnderecoService _enderecoService;

        public EnderecoController(EnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _enderecoService.ListarTodosAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var endereco = await _enderecoService.BuscarPorIdAsync(id.Value);
            if (endereco == null) return NotFound();
            return View(endereco);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Local,Bairro,Cep,Complemento,UsuarioId")] Endereco endereco)
        {
            if (ModelState.IsValid)
            {
                var (resultado, erro) = await _enderecoService.CriarAsync(endereco);
                if (erro != null)
                {
                    ModelState.AddModelError("UsuarioId", erro);
                    return View(endereco);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(endereco);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var endereco = await _enderecoService.BuscarPorIdAsync(id.Value);
            if (endereco == null) return NotFound();
            return View(endereco);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Local,Bairro,Cep,Complemento")] Endereco endereco)
        {
            if (id != endereco.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var resultado = await _enderecoService.EditarAsync(id, endereco);
                    if (resultado == null) return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_enderecoService.Existe(endereco.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(endereco);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var endereco = await _enderecoService.BuscarPorIdAsync(id.Value);
            if (endereco == null) return NotFound();
            return View(endereco);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _enderecoService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> BuscarUsuarioPorCpf(string cpf)
        {
            var usuario = await _enderecoService.BuscarUsuarioPorCpfAsync(cpf);
            if (usuario == null) return Json(new { encontrado = false });
            return Json(new { encontrado = true, id = usuario.Id, nome = usuario.Nome });
        }

        [HttpGet]
        public async Task<IActionResult> BuscarUsuarioPorId(Guid id)
        {
            var usuario = await _enderecoService.BuscarUsuarioPorIdAsync(id);
            if (usuario == null) return Json(new { encontrado = false });
            return Json(new { encontrado = true, id = usuario.Id, nome = usuario.Nome });
        }
    }
}