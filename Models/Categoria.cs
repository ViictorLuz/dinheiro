using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dinheiro.Models
{
	public class Categoria
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "O título é obrigatório.")]
		public string Titulo { get; set; }
	}
}