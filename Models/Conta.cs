using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dinheiro.Models
{
	public class Conta
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "O nome da conta é obrigatório.")]
		[StringLength(50)]
		public string Nome { get; set; }

		[Required(ErrorMessage = "O saldo inicial é obrigatório.")]
		[Column(TypeName = "decimal(18, 2)")]
		public decimal SaldoInicial { get; set; }

		[Required]
		public TipoConta Tipo { get; set; }
	}

	public enum TipoConta
	{
		ContaCorrente,
		Poupanca,
		CartaoDeCredito,
		Investimento,
		Dinheiro
	}
}