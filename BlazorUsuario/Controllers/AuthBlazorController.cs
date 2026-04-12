using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace BlazorUsuario.Controllers
{
    [Route("auth")]
    public class AuthBlazorController : Controller
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UsuarioService _usuarioService;

        public AuthBlazorController(SignInManager<Usuario> signInManager, UsuarioService usuarioService)
        {
            _signInManager = signInManager;
            _usuarioService = usuarioService;
        }

        [HttpPost("blazor-login")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string senha)
        {
            var result = await _signInManager.PasswordSignInAsync(email, senha, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
                return Redirect("/");
            return Redirect("/login?erro=1");
        }

        [HttpPost("blazor-cadastro")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Cadastro(
            [FromForm] string nome,
            [FromForm] string email,
            [FromForm] string cpf,
            [FromForm] string dataNascimento,
            [FromForm] string senha,
            [FromForm] string confirmarSenha)
        {
            if (senha != confirmarSenha)
                return Redirect("/cadastro?erro=senhas");

            var usuario = new Usuario
            {
                UserName = email,
                Email = email,
                Nome = nome,
                Cpf = cpf,
                DataNascimento = DateTime.TryParse(dataNascimento, out var dt) ? dt : DateTime.Today
            };

            var result = await _usuarioService.CriarAsync(usuario, senha, role: "Usuario");
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: true);
                return Redirect("/");
            }

            var erros = string.Join("|", result.Errors.Select(e => e.Description));
            return Redirect($"/cadastro?erro={Uri.EscapeDataString(erros)}");
        }

        [HttpPost("blazor-logout")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/login");
        }
    }
}
