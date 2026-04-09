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

        public async Task<IActionResult> Index(Guid? usuarioId)
        {
            IEnumerable<Endereco> enderecos;
            if (usuarioId.HasValue)
            {
                enderecos = await _enderecoService.ListarPorUsuarioAsync(usuarioId.Value);
                var usuario = await _enderecoService.BuscarUsuarioPorIdAsync(usuarioId.Value);
                ViewBag.UsuarioId = usuarioId;
                ViewBag.UsuarioNome = usuario?.Nome ?? "Usuário";
            }
            else
            {
                enderecos = await _enderecoService.ListarTodosAsync();
                ViewBag.UsuarioId = null;
                ViewBag.UsuarioNome = null;
            }
            return View(enderecos);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var endereco = await _enderecoService.BuscarPorIdAsync(id.Value);
            if (endereco == null) return NotFound();
            return View(endereco);
        }

        public IActionResult Create(Guid? usuarioId)
        {
            var endereco = new Endereco();
            if (usuarioId.HasValue)
                endereco.UsuarioId = usuarioId.Value;
            ViewBag.UsuarioId = usuarioId;
            return View(endereco);
        }

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
                    ViewBag.UsuarioId = endereco.UsuarioId;
                    return View(endereco);
                }
                return RedirectToAction(nameof(Index), new { usuarioId = endereco.UsuarioId });
            }
            ViewBag.UsuarioId = endereco.UsuarioId;
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Local,Bairro,Cep,Complemento,UsuarioId")] Endereco endereco)
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
                return RedirectToAction(nameof(Index), new { usuarioId = endereco.UsuarioId });
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
        public async Task<IActionResult> DeleteConfirmed(Guid id, Guid? usuarioId)
        {
            await _enderecoService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index), new { usuarioId });
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
