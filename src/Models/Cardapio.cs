using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Cardapio
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime Data { get; set; }

        public Cardapio()
        {
            Id = Guid.NewGuid();
        }

        public Cardapio(DateTime data)
        {
            Id = Guid.NewGuid();
            Data = data;
        }
    }
}