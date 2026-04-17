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

        public async Task<ResultadoConflito> VerificarConflitoAsync(Guid mesaId, DateTime inicio, DateTime fim, Guid? ignorarId = null)
        {
            var query = _context.Reservas.Where(r =>
                r.MesaId == mesaId &&
                r.Status != Status.Cancelado &&
                r.HorarioInicio < fim &&
                r.HorarioFim > inicio);

            if (ignorarId.HasValue)
                query = query.Where(r => r.Id != ignorarId.Value);

            var conflitantes = await query
                .Select(r => new ConflitoPeriodo
                {
                    HorarioInicio = r.HorarioInicio,
                    HorarioFim = r.HorarioFim
                })
                .ToListAsync();

            return new ResultadoConflito
            {
                TemConflito = conflitantes.Any(),
                HorariosOcupados = conflitantes
            };
        }

        public async Task<(Reserva? reserva, string? erro)> CriarAsync(Reserva reserva)
        {
            var hora = reserva.HorarioInicio.Hour;
            var minuto = reserva.HorarioInicio.Minute;

            bool eJantar = hora >= 19 && hora < 22;
            if (!eJantar)
                return (null, "Reservas só podem ser feitas para o jantar (19h–22h).");

            var agora = DateTime.Now;
            if (reserva.HorarioInicio.Date != agora.Date)
                return (null, "Reservas só podem ser feitas para o dia atual.");

            var antecedenciaMinima = reserva.HorarioInicio.AddHours(-2);
            if (agora > antecedenciaMinima)
                return (null, $"Reservas devem ser feitas com pelo menos 2 horas de antecedência. Horário limite para este turno: {antecedenciaMinima:HH:mm}.");

            var mesaExiste = await _context.Mesas.AnyAsync(m => m.Id == reserva.MesaId);
            if (!mesaExiste)
                return (null, "Mesa não encontrada.");

            var usuarioExiste = await _context.Users.AnyAsync(u => u.Id == reserva.UsuarioId);
            if (!usuarioExiste)
                return (null, "Usuário não encontrado.");

            var resultado = await VerificarConflitoAsync(reserva.MesaId, reserva.HorarioInicio, reserva.HorarioFim);
            if (resultado.TemConflito)
                return (null, "Mesa já reservada para este horário.");

            reserva.Id = Guid.NewGuid();
            _context.Add(reserva);
            await _context.SaveChangesAsync();
            return (reserva, null);
        }

        public async Task<Reserva?> EditarAsync(Guid id, Reserva reserva)
        {
            var existente = await _context.Reservas.FindAsync(id);
            if (existente == null) return null;

            existente.Data = reserva.Data;
            existente.HorarioInicio = reserva.HorarioInicio;
            existente.HorarioFim = reserva.HorarioFim;
            existente.NumeroPessoas = reserva.NumeroPessoas;
            existente.MesaId = reserva.MesaId;
            existente.UsuarioId = reserva.UsuarioId;
            existente.Status = reserva.Status;

            await _context.SaveChangesAsync();
            return existente;
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
