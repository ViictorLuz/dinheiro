using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dinheiro.Models
{
	public class Meta
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "O título é obrigatório.")]
		[StringLength(100)]
		public string Titulo { get; set; }

		[Required(ErrorMessage = "O valor alvo é obrigatório.")]
		[Range(1.00, double.MaxValue, ErrorMessage = "O valor alvo deve ser maior que zero.")]
		[Column(TypeName = "decimal(18, 2)")]
		public decimal ValorAlvo { get; set; }

		[DataType(DataType.Date)]
		public DateOnly? DataFinalPrevista { get; set; }

		public ICollection<Aporte>? Aportes { get; set; }
	}
}