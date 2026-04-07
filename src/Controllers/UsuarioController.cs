using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: Usuario — apenas Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _usuarioService.ListarTodosAsync());
        }

        // GET: Usuario/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var usuario = await _usuarioService.BuscarPorIdAsync(id.Value);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // GET: Usuario/Edit/5 — Admin ou o próprio usuário
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var usuario = await _usuarioService.BuscarPorIdAsync(id.Value);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Cpf,DataNascimento,Email")] Usuario usuario)
        {
            if (id != usuario.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existente = await _usuarioService.BuscarPorIdAsync(id);
                if (existente == null) return NotFound();

                existente.Nome = usuario.Nome;
                existente.Cpf = usuario.Cpf;
                existente.DataNascimento = usuario.DataNascimento;
                existente.Email = usuario.Email;
                existente.UserName = usuario.Email;

                var resultado = await _usuarioService.EditarAsync(existente);
                if (!resultado.Succeeded)
                {
                    foreach (var erro in resultado.Errors)
                        ModelState.AddModelError(string.Empty, erro.Description);
                    return View(usuario);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuario/Delete/5 — apenas Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var usuario = await _usuarioService.BuscarPorIdAsync(id.Value);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _usuarioService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuario/AlterarSenha
        public async Task<IActionResult> AlterarSenha(Guid? id)
        {
            if (id == null) return NotFound();
            var usuario = await _usuarioService.BuscarPorIdAsync(id.Value);
            if (usuario == null) return NotFound();
            return View(new AlterarSenhaViewModel { Id = usuario.Id });
        }

        // POST: Usuario/AlterarSenha
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var resultado = await _usuarioService.AlterarSenhaAsync(model.Id, model.SenhaAtual, model.NovaSenha);
            if (!resultado.Succeeded)
            {
                foreach (var erro in resultado.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}