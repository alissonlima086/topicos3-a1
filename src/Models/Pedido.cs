using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models
{
    public class Pedido
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        [Required]
        public float PrecoTotal { get; set; }

        public float TaxaEntrega { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        public Usuario? Usuario { get; set; }
        public Atendimento? Atendimento { get; set; }
        public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();

        public Pedido()
        {
            Id = Guid.NewGuid();
            DataHora = DateTime.Now;
            Status = Status.Pendente;
        }

        public void AtualizarStatus(Status novoStatus)
        {
            Status = novoStatus;
        }
    }
}