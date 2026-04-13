using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Services;

namespace BlazorUsuario.Controllers
{
    [Route("auth")]
    public class AuthBlazorController : Controller
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UsuarioService _usuarioService;

        public AuthBlazorController(
            SignInManager<Usuario> signInManager,
            UsuarioService usuarioService)
        {
            _signInManager = signInManager;
            _usuarioService = usuarioService;
        }

        [HttpPost("blazor-login")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login(
            [FromForm] string email,
            [FromForm] string senha,
            [FromForm] string? returnUrl = null)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, senha, false);

                if (result.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("nome", user.Nome ?? ""),
                        new Claim(ClaimTypes.Email, user.Email ?? "")
                    };

                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: true, claims);

                    // Redireciona para returnUrl se for local e válida
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return Redirect("/");
                }
            }

            var erro = $"/login?erro=1{(!string.IsNullOrEmpty(returnUrl) ? $"&returnUrl={Uri.EscapeDataString(returnUrl)}" : "")}";
            return Redirect(erro);
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
                var claims = new List<Claim>
                {
                    new Claim("nome", usuario.Nome ?? ""),
                    new Claim(ClaimTypes.Email, usuario.Email ?? "")
                };

                await _signInManager.SignInWithClaimsAsync(usuario, isPersistent: true, claims);
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
