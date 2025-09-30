using Dinheiro.Models;
using Microsoft.EntityFrameworkCore;

namespace Dinheiro.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<Transacao> Transacoes { get; set; }
		public DbSet<Categoria> Categorias { get; set; }
		public DbSet<TransacaoRecorrente> TransacoesRecorrentes { get; set; }
		public DbSet<Meta> Metas { get; set; }
		public DbSet<Aporte> Aportes { get; set; }
		public DbSet<Conta> Contas { get; set; }
	}
}