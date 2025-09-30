using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dinheiro.Models
{
	public class Transacao
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "A descrição é obrigatória.")]
		[StringLength(75, ErrorMessage = "A descrição não pode ter mais de 75 caracteres.")]
		public string Descricao { get; set; }

		[Required(ErrorMessage = "O valor é obrigatório.")]
		[Range(0.01, 1000000.00, ErrorMessage = "O valor deve ser maior que R$ 0,00.")]
		[Column(TypeName = "decimal(18, 2)")]
		public decimal Valor { get; set; }

		[DataType(DataType.Date)]
		public DateOnly Data { get; set; } = DateOnly.FromDateTime(DateTime.Now);

		[Required(ErrorMessage = "O tipo é obrigatório.")]
		public TipoTransacao Tipo { get; set; }

		[Display(Name = "Categoria")]
		[Required(ErrorMessage = "A categoria é obrigatória.")]
		public int CategoriaId { get; set; }
		public Categoria? Categoria { get; set; }

		[Display(Name = "Conta")]
		[Required(ErrorMessage = "A conta é obrigatória.")]
		public int ContaId { get; set; }
		public Conta? Conta { get; set; }
	}

	public enum TipoTransacao
	{
		Receita = 1,
		Despesa = 2
	}
}