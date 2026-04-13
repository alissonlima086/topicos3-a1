namespace WebApplication1.Models.ViewModel
{
    public enum PeriodoRelatorio { Semana, Mes, Ano }

    public class FaturamentoTipoDto
    {
        public string TipoAtendimento { get; set; } = string.Empty;
        public float Total { get; set; }
        public int Quantidade { get; set; }
    }

    public class ItemMaisVendidoDto
    {
        public string NomePrato { get; set; } = string.Empty;
        public bool FoiSugestao { get; set; }
        public int TotalVendido { get; set; }
        public float ReceitaTotal { get; set; }
    }

    public class RelatorioViewModel
    {
        public PeriodoRelatorio Periodo { get; set; } = PeriodoRelatorio.Mes;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public List<FaturamentoTipoDto> FaturamentoPorTipo { get; set; } = new();
        public float FaturamentoTotal { get; set; }
        public int TotalPedidos { get; set; }

        public List<ItemMaisVendidoDto> ItensMaisVendidos { get; set; } = new();
        public List<ItemMaisVendidoDto> SugestoesMaisVendidas { get; set; } = new();
    }
}
