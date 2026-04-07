using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models
{
    public class Atendimento
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Status Status { get; set; }

        public Atendimento()
        {
            Id = Guid.NewGuid();
            Status = Status.EmAtendimento;
        }

        public void Finalizar()
        {
            Status = Status.Finalizado;
        }
    }
}