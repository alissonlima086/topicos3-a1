using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Services;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly IReservaService _reservaService;
        private readonly ApplicationDbContext _context;

        public ReservaController(IReservaService reservaService, ApplicationDbContext context)
        {
            _reservaService = reservaService;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _reservaService.ListarTodosAsync());
        }

        public async Task<IActionResult> MinhasReservas(Guid usuarioId)
        {
            return View(await _reservaService.ListarPorUsuarioAsync(usuarioId));
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var reserva = await _reservaService.BuscarPorIdAsync(id.Value);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new ReservaCreateViewModel
            {
                Data = DateTime.Today,
                Mesas = await CarregarMesasAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservaCreateViewModel vm)
        {
            DateTime horarioInicio = default, horarioFim = default;

            if (!string.IsNullOrWhiteSpace(vm.HoraInicio) && TimeSpan.TryParse(vm.HoraInicio, out var tsI))
                horarioInicio = vm.Data.Date + tsI;
            else
                ModelState.AddModelError("HoraInicio", "Horário de início inválido.");

            if (!string.IsNullOrWhiteSpace(vm.HoraFim) && TimeSpan.TryParse(vm.HoraFim, out var tsF))
                horarioFim = vm.Data.Date + tsF;
            else
                ModelState.AddModelError("HoraFim", "Horário de fim inválido.");

            if (horarioFim != default && horarioInicio != default && horarioFim <= horarioInicio)
                ModelState.AddModelError("HoraFim", "O horário de fim deve ser posterior ao de início.");

            if (horarioInicio != default && horarioInicio.Hour < 18)
                ModelState.AddModelError("HoraInicio", "Reservas só podem ser feitas para o jantar (a partir das 18h).");

            Guid? usuarioId = vm.UsuarioId;
            if (usuarioId == null && !string.IsNullOrWhiteSpace(vm.UsuarioCpf))
            {
                var usuario = await _reservaService.BuscarUsuarioPorCpfAsync(vm.UsuarioCpf);
                if (usuario == null)
                    ModelState.AddModelError("UsuarioCpf", "Usuário com este CPF não encontrado.");
                else
                    usuarioId = usuario.Id;
            }

            if (usuarioId == null)
                ModelState.AddModelError("UsuarioId", "Informe o ID ou CPF do usuário.");

            if (!ModelState.IsValid)
            {
                vm.Mesas = await CarregarMesasAsync();
                return View(vm);
            }

            var reserva = new Reserva
            {
                Data = vm.Data.Date,
                HorarioInicio = horarioInicio,
                HorarioFim = horarioFim,
                NumeroPessoas = vm.NumeroPessoas,
                MesaId = vm.MesaId,
                UsuarioId = usuarioId!.Value
            };

            var (resultado, erro) = await _reservaService.CriarAsync(reserva);
            if (erro != null)
            {
                ModelState.AddModelError(string.Empty, erro);
                vm.Mesas = await CarregarMesasAsync();
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> VerificarConflito(Guid mesaId, string data, string horaInicio, string horaFim, Guid? ignorarId = null)
        {
            if (mesaId == Guid.Empty || string.IsNullOrWhiteSpace(data) ||
                string.IsNullOrWhiteSpace(horaInicio) || string.IsNullOrWhiteSpace(horaFim))
                return Json(new { conflito = false });

            if (!DateTime.TryParse(data, out var d) ||
                !TimeSpan.TryParse(horaInicio, out var tsI) ||
                !TimeSpan.TryParse(horaFim, out var tsF))
                return Json(new { conflito = false });

            var inicio = d.Date + tsI;
            var fim = d.Date + tsF;

            if (fim <= inicio)
                return Json(new { conflito = false, erro = "Fim deve ser após início." });

            var resultado = await _reservaService.VerificarConflitoAsync(mesaId, inicio, fim, ignorarId);

            if (resultado.TemConflito)
            {
                var ocupadas = resultado.HorariosOcupados
                    .Select(h => new { ini = h.HorarioInicio.ToString("HH:mm"), fim = h.HorarioFim.ToString("HH:mm") });
                return Json(new { conflito = true, ocupadas });
            }

            return Json(new { conflito = false });
        }

        [HttpGet]
        public async Task<IActionResult> BuscarUsuarioPorCpf(string cpf)
        {
            var usuario = await _reservaService.BuscarUsuarioPorCpfAsync(cpf);
            if (usuario == null) return Json(new { encontrado = false });
            return Json(new { encontrado = true, id = usuario.Id, nome = usuario.Nome });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var reserva = await _reservaService.BuscarPorIdAsync(id.Value);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Data,HorarioInicio,HorarioFim,NumeroPessoas,UsuarioId,MesaId,Status")] Reserva reserva)
        {
            if (id != reserva.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var resultado = await _reservaService.EditarAsync(id, reserva);
                    if (resultado == null) return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_reservaService.Existe(reserva.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reserva);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var reserva = await _reservaService.BuscarPorIdAsync(id.Value);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _reservaService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AtualizarStatus(Guid id, Status status)
        {
            var resultado = await _reservaService.AtualizarStatusAsync(id, status);
            if (resultado == null) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<MesaSelecaoViewModel>> CarregarMesasAsync()
        {
            return await _context.Mesas
                .Select(m => new MesaSelecaoViewModel { Id = m.Id, Numero = m.Numero, Capacidade = m.Capacidade })
                .OrderBy(m => m.Numero)
                .ToListAsync();
        }
    }
}
