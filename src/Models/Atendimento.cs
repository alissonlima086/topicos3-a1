using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models
{
    public abstract class Atendimento
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Status Status { get; set; }

        public Guid PedidoId { get; set; }
        public Pedido? Pedido { get; set; }

        protected Atendimento()
        {
            Id = Guid.NewGuid();
            Status = Status.EmAtendimento;
        }

        public abstract void ProcessarAtendimento();

        public void Finalizar()
        {
            Status = Status.Finalizado;
        }
    }
}
