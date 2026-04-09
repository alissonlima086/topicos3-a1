using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models
{
    public class Cardapio
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        public Turno Turno { get; set; }

        public ICollection<ItemCardapio> Itens { get; set; } = new List<ItemCardapio>();

        public Cardapio()
        {
            Id = Guid.NewGuid();
        }
    }
}