namespace WebApplication1.Models.ViewModels
{
    public class PratoEditViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public float PrecoBase { get; set; }
        public bool Ativo { get; set; }
        public WebApplication1.Models.Enums.Turno Turno { get; set; }
        public List<IngredienteSelecaoViewModel> Ingredientes { get; set; } = new();
    }

    public class IngredienteSelecaoViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Selecionado { get; set; }
    }
}
