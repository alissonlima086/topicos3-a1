namespace WebApplication1.Models.ViewModels
{
    public class PratoEditViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public float PrecoBase { get; set; }
        public bool Ativo { get; set; }

        // Todos os ingredientes disponíveis com flag se está selecionado
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
