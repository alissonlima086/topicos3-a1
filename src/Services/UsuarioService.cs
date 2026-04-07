using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> ListarTodosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> CriarAsync(Usuario usuario)
        {
            usuario.Id = Guid.NewGuid();
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> EditarAsync(Guid id, Usuario usuario)
        {
            var existente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (existente == null) return null;

            usuario.Senha = existente.Senha;

            _context.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AlterarSenhaAsync(Guid id, string senhaAtual, string novaSenha)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(senhaAtual, usuario.Senha))
                return false;

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            _context.Update(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Existe(Guid id)
        {
            return _context.Usuarios.Any(u => u.Id == id);
        }
    }
}