using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{

public class Atendimento
{
    [Key]
    public Guid Id { get; set; }

    public Atendimento()
    {
        Id = Guid.NewGuid();
    }
}

public class AtendimentoPresencial : Atendimento
{
    [Required]
    public int NumeroMesa { get; set; }
}

public class AtendimentoDeliveryProprio : Atendimento
{
    [Required]
    public float TaxaFixa { get; set; }
}

public class AtendimentoDeliveryApp : Atendimento
{
    [Required]
    public float ComissaoPorcentagem { get; set; }

    public float TaxaAdicional { get; set; }
}
}