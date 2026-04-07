using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UsuarioService _usuarioService;

        public AuthController(SignInManager<Usuario> signInManager, UsuarioService usuarioService)
        {
            _signInManager = signInManager;
            _usuarioService = usuarioService;
        }

        // GET: Auth/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var resultado = await _signInManager.PasswordSignInAsync(
                model.Email, model.Senha, model.LembrarMe, lockoutOnFailure: false);

            if (resultado.Succeeded)
                return LocalRedirect(returnUrl ?? "/");

            ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
            return View(model);
        }

        // GET: Auth/Cadastro
        public IActionResult Cadastro()
        {
            return View();
        }

        // POST: Auth/Cadastro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(CadastroViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = new Usuario
            {
                Nome = model.Nome,
                Email = model.Email,
                Cpf = model.Cpf,
                DataNascimento = model.DataNascimento
            };

            var resultado = await _usuarioService.CriarAsync(usuario, model.Senha, role: "Usuario");

            if (resultado.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var erro in resultado.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);

            return View(model);
        }

        // POST: Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}