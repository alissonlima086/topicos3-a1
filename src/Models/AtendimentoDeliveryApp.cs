using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AtendimentoDeliveryApp : Atendimento
    {
        [Required]
        [MaxLength(100)]
        public string NomeApp { get; set; } = string.Empty;

        [Required]
        public float ComissaoPorcentagem { get; set; }

        public float TaxaAdicional { get; set; }

        public AtendimentoDeliveryApp() : base() { }

        public AtendimentoDeliveryApp(string nomeApp, float comissaoPorcentagem, float taxaAdicional = 0) : base()
        {
            NomeApp = nomeApp;
            ComissaoPorcentagem = comissaoPorcentagem;
            TaxaAdicional = taxaAdicional;
        }

        public float CalcularValorLiquidoRestaurante(float valorPedido)
            => valorPedido * (1 - ComissaoPorcentagem) - TaxaAdicional;

        public override void ProcessarAtendimento()
        {
            float valorBruto = Pedido?.PrecoTotal ?? 0;
            float valorLiquido = CalcularValorLiquidoRestaurante(valorBruto);

            Console.WriteLine($"[Delivery App - {NomeApp}] " +
                $"Comissão: {ComissaoPorcentagem * 100:F0}% + R$ {TaxaAdicional:F2}. " +
                $"Valor bruto: R$ {valorBruto:F2} | Valor líquido p/ restaurante: R$ {valorLiquido:F2}.");
        }
    }
}
