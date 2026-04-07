using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class UsuarioService
    {
        private readonly UserManager<Usuario> _userManager;

        public UsuarioService(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<Usuario>> ListarTodosAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<Usuario?> BuscarPorIdAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<IdentityResult> CriarAsync(Usuario usuario, string senha, string role = "Usuario")
        {
            usuario.UserName = usuario.Email;
            var resultado = await _userManager.CreateAsync(usuario, senha);
            if (resultado.Succeeded)
                await _userManager.AddToRoleAsync(usuario, role);
            return resultado;
        }

        public async Task<IdentityResult> EditarAsync(Usuario usuario)
        {
            return await _userManager.UpdateAsync(usuario);
        }

        public async Task<IdentityResult> ExcluirAsync(Guid id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario == null) return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            return await _userManager.DeleteAsync(usuario);
        }

        public async Task<IdentityResult> AlterarSenhaAsync(Guid id, string senhaAtual, string novaSenha)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario == null) return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            return await _userManager.ChangePasswordAsync(usuario, senhaAtual, novaSenha);
        }

        public async Task<string?> ObterRoleAsync(Usuario usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            return roles.FirstOrDefault();
        }

        public bool Existe(Guid id)
        {
            return _userManager.Users.Any(u => u.Id == id);
        }
    }
}