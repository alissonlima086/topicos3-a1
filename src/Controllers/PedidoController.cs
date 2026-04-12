using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly IConfiguracaoDeliveryService _deliveryService;
        private readonly ICardapioService _cardapioService;
        private readonly ApplicationDbContext _context;

        public PedidoController(IPedidoService pedidoService,
            IConfiguracaoDeliveryService deliveryService,
            ICardapioService cardapioService,
            ApplicationDbContext context)
        {
            _pedidoService = pedidoService;
            _deliveryService = deliveryService;
            _cardapioService = cardapioService;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Itens)
                .OrderByDescending(p => p.DataHora)
                .ToListAsync();
            return View(pedidos);
        }

        public async Task<IActionResult> MeusPedidos(Guid usuarioId)
        {
            return View(await _pedidoService.ListarPorUsuarioAsync(usuarioId));
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var pedido = await _pedidoService.BuscarPorIdAsync(id.Value);
            if (pedido == null) return NotFound();
            return View(pedido);
        }

        public async Task<IActionResult> Create()
        {
            await CarregarViewBagCreate();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Guid usuarioId,
            Guid? configuracaoDeliveryId,
            List<ItemCardapioSelecionadoPedido> itens)
        {
            var validos = itens?.Where(i => i.Quantidade > 0).ToList() ?? new();
            if (!validos.Any())
            {
                ModelState.AddModelError("", "Selecione pelo menos um item do cardápio.");
                await CarregarViewBagCreate();
                return View();
            }

            // Taxa automática
            float taxaEntrega = 0;
            if (configuracaoDeliveryId.HasValue)
            {
                var config = await _deliveryService.BuscarPorIdAsync(configuracaoDeliveryId.Value);
                if (config != null)
                    taxaEntrega = config.TaxaFixaProprio ?? config.TaxaAdicionalApp ?? 0;
            }

            // Resolve pratos do cardápio
            var ids = validos.Select(v => v.ItemCardapioId).ToList();
            var itensCardapio = await _context.ItensCardapio
                .Include(ic => ic.Prato)
                .Where(ic => ids.Contains(ic.Id))
                .ToListAsync();

            var itensPedido = validos
                .Select(v =>
                {
                    var ic = itensCardapio.FirstOrDefault(x => x.Id == v.ItemCardapioId);
                    if (ic?.Prato == null) return null;
                    var preco = ic.IsSugestao ? ic.Prato.PrecoBase * 0.8f : ic.Prato.PrecoBase;
                    return new ItemPedido
                    {
                        NomePrato = ic.Prato.Nome,
                        PrecoUnitario = preco,
                        Quantidade = v.Quantidade,
                        FoiSugestao = ic.IsSugestao,
                        Observacao = v.Observacao ?? string.Empty
                    };
                })
                .Where(i => i != null)
                .Cast<ItemPedido>()
                .ToList();

            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                TaxaEntrega = taxaEntrega,
                PrecoTotal = itensPedido.Sum(i => i.PrecoUnitario * i.Quantidade) + taxaEntrega,
                Itens = itensPedido
            };

            var (resultado, erro) = await _pedidoService.CriarAsync(pedido);
            if (erro != null)
            {
                ModelState.AddModelError(string.Empty, erro);
                await CarregarViewBagCreate();
                return View();
            }
            return RedirectToAction(nameof(Details), new { id = resultado!.Id });
        }

        private async Task CarregarViewBagCreate()
        {
            var turno = TurnoAtual();
            var cardapio = await _cardapioService.BuscarAtualAsync(DateTime.Today, turno);
            ViewBag.Cardapio = cardapio;
            ViewBag.TurnoAtual = turno.ToString();

            var configs = (await _deliveryService.ListarTodosAsync()).Where(c => c.Ativo).ToList();
            ViewBag.ConfiguracoesDelivery = new SelectList(configs, "Id", "NomeApp");
            ViewBag.Configuracoes = configs;
        }

        private static Turno TurnoAtual()
        {
            var h = DateTime.Now.Hour;
            return h switch { >= 6 and < 12 => Turno.Manha, >= 12 and < 18 => Turno.Tarde, >= 18 => Turno.Noite, _ => Turno.Madrugada };
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var pedido = await _pedidoService.BuscarPorIdAsync(id.Value);
            if (pedido == null) return NotFound();
            return View(pedido);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,DataHora,PrecoTotal,TaxaEntrega,Status,UsuarioId")] Pedido pedido)
        {
            if (id != pedido.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { await _pedidoService.EditarAsync(id, pedido); }
                catch (DbUpdateConcurrencyException)
                { if (!_pedidoService.Existe(pedido.Id)) return NotFound(); throw; }
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var pedido = await _pedidoService.BuscarPorIdAsync(id.Value);
            if (pedido == null) return NotFound();
            return View(pedido);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _pedidoService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> AtualizarStatus(Guid id, Status status)
        {
            var resultado = await _pedidoService.AtualizarStatusAsync(id, status);
            if (resultado == null) return NotFound();
            return RedirectToAction(nameof(Index));
        }
    }

    // DTO local para binding do cardápio no pedido
    public class ItemCardapioSelecionadoPedido
    {
        public Guid ItemCardapioId { get; set; }
        public int Quantidade { get; set; }
        public string? Observacao { get; set; }
    }
}
