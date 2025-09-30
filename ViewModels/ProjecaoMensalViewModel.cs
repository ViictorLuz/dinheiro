namespace Dinheiro.ViewModels
{
	public class ProjecaoMensalViewModel
	{
		public string MesAno { get; set; }
		public decimal SaldoInicial { get; set; }
		public decimal TotalReceitas { get; set; }
		public decimal TotalDespesas { get; set; }
		public decimal DespesasSimuladas { get; set; }
		public decimal SaldoFinal { get; set; }
	}
}