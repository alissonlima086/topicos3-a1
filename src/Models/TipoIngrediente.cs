using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TipoIngrediente
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public TipoIngrediente()
        {
            Id = Guid.NewGuid();
        }

        public TipoIngrediente(string nome, string descricao)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Descricao = descricao;
        }
    }
}