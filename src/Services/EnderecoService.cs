using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class EnderecoService
    {
        private readonly ApplicationDbContext _context;

        public EnderecoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Endereco>> ListarTodosAsync()
        {
            return await _context.Enderecos.ToListAsync();
        }

        public async Task<Endereco?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<(Endereco? endereco, string? erro)> CriarAsync(Endereco endereco)
        {
            var usuarioExiste = await _context.Users.AnyAsync(u => u.Id == endereco.UsuarioId);
            if (!usuarioExiste)
                return (null, "Usuário não encontrado.");

            endereco.Id = Guid.NewGuid();
            _context.Add(endereco);
            await _context.SaveChangesAsync();
            return (endereco, null);
        }

        public async Task<Endereco?> EditarAsync(Guid id, Endereco endereco)
        {
            if (!Existe(id)) return null;
            _context.Update(endereco);
            await _context.SaveChangesAsync();
            return endereco;
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var endereco = await _context.Enderecos.FindAsync(id);
            if (endereco == null) return false;
            _context.Enderecos.Remove(endereco);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Usuario?> BuscarUsuarioPorCpfAsync(string cpf)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Cpf == cpf);
        }

        public async Task<Usuario?> BuscarUsuarioPorIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public bool Existe(Guid id)
        {
            return _context.Enderecos.Any(e => e.Id == id);
        }
    }
}