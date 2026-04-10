using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class AtendimentoPresencial : Atendimento
    {
        [Required]
        public Guid MesaId { get; set; }
        public Mesa? Mesa { get; set; }

        public AtendimentoPresencial() : base() { }

        public AtendimentoPresencial(Guid mesaId) : base()
        {
            MesaId = mesaId;
        }

        public override void ProcessarAtendimento()
        {
            Console.WriteLine($"[Presencial] Atendendo mesa {MesaId}. Sem taxas adicionais.");
        }
    }
}
