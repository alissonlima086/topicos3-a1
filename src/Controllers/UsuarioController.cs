using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            return View(await _usuarioService.ListarTodosAsync());
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var usuario = await _usuarioService.BuscarPorIdAsync(id.Value);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,Senha,Cpf,DataNascimento,Funcao")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                await _usuarioService.CriarAsync(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuario/Edit/5
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Email,Senha,Cpf,DataNascimento,Funcao")] Usuario usuario)
        {
            if (id != usuario.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var resultado = await _usuarioService.EditarAsync(id, usuario);
                    if (resultado == null) return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_usuarioService.Existe(usuario.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuario/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _usuarioService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuario/AlterarSenha/5
        public async Task<IActionResult> AlterarSenha(Guid? id)
        {
            if (id == null) return NotFound();
            var usuario = await _usuarioService.BuscarPorIdAsync(id.Value);
            if (usuario == null) return NotFound();
            return View(new AlterarSenhaViewModel { Id = usuario.Id });
        }

        // POST: Usuario/AlterarSenha/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var sucesso = await _usuarioService.AlterarSenhaAsync(model.Id, model.SenhaAtual, model.NovaSenha);

            if (!sucesso)
            {
                ModelState.AddModelError("SenhaAtual", "Senha atual incorreta.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}