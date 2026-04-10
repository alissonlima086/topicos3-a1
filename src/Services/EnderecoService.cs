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
            return await _context.Enderecos.Include(e => e.Usuario).ToListAsync();
        }

        public async Task<IEnumerable<Endereco>> ListarPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Enderecos
                .Where(e => e.UsuarioId == usuarioId)
                .OrderByDescending(e => e.Principal)
                .ToListAsync();
        }

        public async Task<Endereco?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Enderecos
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<(Endereco? endereco, string? erro)> CriarAsync(Endereco endereco)
        {
            var usuarioExiste = await _context.Users.AnyAsync(u => u.Id == endereco.UsuarioId);
            if (!usuarioExiste)
                return (null, "Usuário não encontrado.");

            if (endereco.Principal)
                await DesmarcarPrincipaisAsync(endereco.UsuarioId, null);

            var temOutros = await _context.Enderecos.AnyAsync(e => e.UsuarioId == endereco.UsuarioId);
            if (!temOutros)
                endereco.Principal = true;

            endereco.Id = Guid.NewGuid();
            _context.Add(endereco);
            await _context.SaveChangesAsync();
            return (endereco, null);
        }

        public async Task<Endereco?> EditarAsync(Guid id, Endereco endereco)
        {
            if (!Existe(id)) return null;

            if (endereco.Principal)
                await DesmarcarPrincipaisAsync(endereco.UsuarioId, id);

            _context.Update(endereco);
            await _context.SaveChangesAsync();
            return endereco;
        }

        public async Task<bool> DefinirPrincipalAsync(Guid enderecoId)
        {
            var endereco = await _context.Enderecos.FindAsync(enderecoId);
            if (endereco == null) return false;

            await DesmarcarPrincipaisAsync(endereco.UsuarioId, enderecoId);
            endereco.Principal = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task DesmarcarPrincipaisAsync(Guid usuarioId, Guid? exceto)
        {
            var principais = await _context.Enderecos
                .Where(e => e.UsuarioId == usuarioId && e.Principal && e.Id != exceto)
                .ToListAsync();
            foreach (var e in principais)
                e.Principal = false;
            // SaveChanges será chamado pelo método que chamou este
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var endereco = await _context.Enderecos.FindAsync(id);
            if (endereco == null) return false;

            var eraPrincipal = endereco.Principal;
            var usuarioId = endereco.UsuarioId;

            _context.Enderecos.Remove(endereco);
            await _context.SaveChangesAsync();

            if (eraPrincipal)
            {
                var proximo = await _context.Enderecos
                    .Where(e => e.UsuarioId == usuarioId)
                    .FirstOrDefaultAsync();
                if (proximo != null)
                {
                    proximo.Principal = true;
                    await _context.SaveChangesAsync();
                }
            }

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
