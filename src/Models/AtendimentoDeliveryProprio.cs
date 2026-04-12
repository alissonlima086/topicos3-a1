using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AtendimentoDeliveryProprio : Atendimento
    {
        [Required]
        public float TaxaFixa { get; set; }

        public Guid? EnderecoEntregaId { get; set; }
        public Endereco? EnderecoEntrega { get; set; }

        public AtendimentoDeliveryProprio() : base() { }

        public AtendimentoDeliveryProprio(float taxaFixa, Guid? enderecoEntregaId = null) : base()
        {
            TaxaFixa = taxaFixa;
            EnderecoEntregaId = enderecoEntregaId;
        }

        public override void ProcessarAtendimento()
        {
            if (Pedido != null)
                Pedido.TaxaEntrega = TaxaFixa;

            Console.WriteLine($"[Delivery Próprio] Taxa fixa de R$ {TaxaFixa:F2} aplicada. Motoboy do restaurante acionado.");
        }
    }
}
