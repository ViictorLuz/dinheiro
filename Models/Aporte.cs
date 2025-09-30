using Dinheiro.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dinheiro.Models
{
	public class Aporte
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "O valor do aporte é obrigatório.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
		[Column(TypeName = "decimal(18, 2)")]
		public decimal Valor { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateOnly Data { get; set; }

		[Required]
		public int MetaId { get; set; }
		public Meta? Meta { get; set; }
	}
}