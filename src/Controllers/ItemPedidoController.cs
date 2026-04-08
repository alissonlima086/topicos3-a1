using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ItemPedidoController : Controller
    {
        private readonly IItemPedidoService _itemPedidoService;
        private readonly IPratoService _pratoService;

        public ItemPedidoController(IItemPedidoService itemPedidoService, IPratoService pratoService)
        {
            _itemPedidoService = itemPedidoService;
            _pratoService = pratoService;
        }

        public async Task<IActionResult> Index(Guid pedidoId)
        {
            return View(await _itemPedidoService.ListarPorPedidoAsync(pedidoId));
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var item = await _itemPedidoService.BuscarPorIdAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create(Guid pedidoId)
        {
            var pratos = await _pratoService.ListarAtivosAsync();
            var vm = new ItemPedidoCreateViewModel
            {
                PedidoId = pedidoId,
                PratosDisponiveis = pratos.Select(p => new PratoOpcaoViewModel
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    PrecoBase = p.PrecoBase
                }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemPedidoCreateViewModel vm)
        {
            if (vm.PratoSelecionadoId == null)
            {
                ModelState.AddModelError("PratoSelecionadoId", "Selecione um prato.");
            }

            if (ModelState.IsValid)
            {
                // Busca o prato para pegar o nome atualizado
                var prato = await _pratoService.BuscarPorIdAsync(vm.PratoSelecionadoId!.Value);
                if (prato == null)
                {
                    ModelState.AddModelError("PratoSelecionadoId", "Prato não encontrado.");
                }
                else
                {
                    var item = new ItemPedido
                    {
                        PedidoId = vm.PedidoId,
                        NomePrato = prato.Nome,
                        Quantidade = vm.Quantidade,
                        PrecoUnitario = vm.PrecoUnitario,
                        FoiSugestao = vm.FoiSugestao,
                        Observacao = vm.Observacao
                    };

                    var (resultado, erro) = await _itemPedidoService.AdicionarAsync(item);
                    if (erro != null)
                    {
                        ModelState.AddModelError(string.Empty, erro);
                    }
                    else
                    {
                        return RedirectToAction("Details", "Pedido", new { id = vm.PedidoId });
                    }
                }
            }

            // Recarrega pratos se houver erro
            var pratos = await _pratoService.ListarAtivosAsync();
            vm.PratosDisponiveis = pratos.Select(p => new PratoOpcaoViewModel
            {
                Id = p.Id,
                Nome = p.Nome,
                PrecoBase = p.PrecoBase
            }).ToList();
            return View(vm);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var item = await _itemPedidoService.BuscarPorIdAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,PedidoId,NomePrato,Quantidade,PrecoUnitario,FoiSugestao,Observacao")] ItemPedido item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var resultado = await _itemPedidoService.EditarAsync(id, item);
                if (resultado == null) return NotFound();
                return RedirectToAction("Details", "Pedido", new { id = item.PedidoId });
            }
            return View(item);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var item = await _itemPedidoService.BuscarPorIdAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var item = await _itemPedidoService.BuscarPorIdAsync(id);
            var pedidoId = item?.PedidoId;
            await _itemPedidoService.RemoverAsync(id);
            return RedirectToAction("Details", "Pedido", new { id = pedidoId });
        }
    }
}
