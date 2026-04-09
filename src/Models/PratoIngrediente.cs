namespace WebApplication1.Models
{
    public class PratoIngrediente
    {
        public Guid PratoId { get; set; }
        public Prato Prato { get; set; } = null!;

        public Guid IngredienteId { get; set; }
        public Ingrediente Ingrediente { get; set; } = null!;
    }
}
