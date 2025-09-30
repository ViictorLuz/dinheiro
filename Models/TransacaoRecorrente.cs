using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dinheiro.Models
{
	public class TransacaoRecorrente
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "A descrição é obrigatória.")]
		[StringLength(75)]
		public string Descricao { get; set; }

		[Required(ErrorMessage = "O valor é obrigatório.")]
		[Range(0.01, 1000000.00, ErrorMessage = "O valor deve ser maior que R$ 0,00.")]
		[Column(TypeName = "decimal(18, 2)")]
		public decimal Valor { get; set; }

		[Required]
		public TipoTransacao Tipo { get; set; }

		[Required(ErrorMessage = "O dia do mês é obrigatório.")]
		[Range(1, 31, ErrorMessage = "O dia deve ser entre 1 e 31.")]
		public int DiaDoMes { get; set; }

		[Required(ErrorMessage = "A data de início é obrigatória.")]
		[DataType(DataType.Date)]
		public DateOnly DataInicio { get; set; }

		[DataType(DataType.Date)]
		public DateOnly? DataFim { get; set; }

		public bool Ativo { get; set; } = true;

		[Required(ErrorMessage = "A categoria é obrigatória.")]
		public int CategoriaId { get; set; }
		public Categoria? Categoria { get; set; }
	}
}