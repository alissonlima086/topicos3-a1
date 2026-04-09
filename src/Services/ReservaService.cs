using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Enums;

namespace WebApplication1.Services
{
    public class ReservaService : IReservaService
    {
        private readonly ApplicationDbContext _context;

        public ReservaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reserva>> ListarTodosAsync()
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Mesa)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> ListarPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Reservas
                .Include(r => r.Mesa)
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Reserva?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Mesa)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<(Reserva? reserva, string? erro)> CriarAsync(Reserva reserva)
        {
            var mesaExiste = await _context.Mesas.AnyAsync(m => m.Id == reserva.MesaId);
            if (!mesaExiste)
                return (null, "Mesa não encontrada.");

            var usuarioExiste = await _context.Users.AnyAsync(u => u.Id == reserva.UsuarioId);
            if (!usuarioExiste)
                return (null, "Usuário não encontrado.");

            var conflito = await _context.Reservas.AnyAsync(r =>
                r.MesaId == reserva.MesaId &&
                r.Status != Status.Cancelado &&
                r.HorarioInicio < reserva.HorarioFim &&
                r.HorarioFim > reserva.HorarioInicio);
            if (conflito)
                return (null, "Mesa já reservada para este horário.");

            reserva.Id = Guid.NewGuid();
            _context.Add(reserva);
            await _context.SaveChangesAsync();
            return (reserva, null);
        }

        public async Task<Reserva?> EditarAsync(Guid id, Reserva reserva)
        {
            if (!Existe(id)) return null;
            _context.Update(reserva);
            await _context.SaveChangesAsync();
            return reserva;
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return false;
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Reserva?> AtualizarStatusAsync(Guid id, Status status)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return null;
            reserva.AtualizarStatus(status);
            await _context.SaveChangesAsync();
            return reserva;
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
            return _context.Reservas.Any(r => r.Id == id);
        }
    }
}