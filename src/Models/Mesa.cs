using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Mesa
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Numero { get; set; }

        [Required]
        public int Capacidade { get; set; }

        [Required]
        public bool Disponivel { get; set; }

        public Mesa()
        {
            Id = Guid.NewGuid();
            Disponivel = true;
        }

        public void Ocupar() => Disponivel = false;
        public void Liberar() => Disponivel = true;
    }
}