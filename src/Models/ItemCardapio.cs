using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ItemCardapio
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CardapioId { get; set; }
        public Cardapio? Cardapio { get; set; }

        [Required]
        public Guid PratoId { get; set; }
        public Prato? Prato { get; set; }

        public bool IsSugestao { get; set; }

        public ItemCardapio()
        {
            Id = Guid.NewGuid();
        }
    }
}
