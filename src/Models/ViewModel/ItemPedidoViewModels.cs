namespace WebApplication1.Models.ViewModels
{
    public class ItemPedidoCreateViewModel
    {
        public Guid PedidoId { get; set; }
        public Guid? PratoSelecionadoId { get; set; }
        public int Quantidade { get; set; } = 1;
        public float PrecoUnitario { get; set; }
        public bool FoiSugestao { get; set; }
        public string Observacao { get; set; } = string.Empty;

        public List<PratoOpcaoViewModel> PratosDisponiveis { get; set; } = new();
    }

    public class PratoOpcaoViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float PrecoBase { get; set; }
    }
}
