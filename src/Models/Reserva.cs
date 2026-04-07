using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Reserva
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        public DateTime HorarioInicio { get; set; }

        [Required]
        public DateTime HorarioFim { get; set; }

        [Required]
        public int NumeroPessoas { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        [Required]
        public Guid MesaId { get; set; }

        public Reserva()
        {
            Id = Guid.NewGuid();
        }
    }
}