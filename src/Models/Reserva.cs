using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;

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

        [Required]
        public Status Status { get; set; }

        public Usuario? Usuario { get; set; }
        public Mesa? Mesa { get; set; }

        public Reserva()
        {
            Id = Guid.NewGuid();
            Status = Status.Pendente;
        }

        public void AtualizarStatus(Status novoStatus)
        {
            Status = novoStatus;
        }
    }
}