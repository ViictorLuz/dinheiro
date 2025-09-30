using Dinheiro.Data;
using Dinheiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dinheiro.Controllers
{
	public class RelatoriosController : Controller
	{
		private readonly ApplicationDbContext _context;

		public RelatoriosController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(DateOnly? dataInicio, DateOnly? dataFim)
		{
			if (!dataInicio.HasValue)
			{
				dataInicio = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
			}
			if (!dataFim.HasValue)
			{
				dataFim = dataInicio.Value.AddMonths(1).AddDays(-1);
			}

			ViewData["dataInicio"] = dataInicio.Value.ToString("yyyy-MM-dd");
			ViewData["dataFim"] = dataFim.Value.ToString("yyyy-MM-dd");

			var transacoesReais = await _context.Transacoes
				.Include(t => t.Categoria)
				.Where(t => t.Data >= dataInicio.Value && t.Data <= dataFim.Value)
				.ToListAsync();

			var regrasRecorrentes = await _context.TransacoesRecorrentes
				.Include(t => t.Categoria)
				.Where(r => r.Ativo && r.DataInicio <= dataFim.Value && (r.DataFim == null || r.DataFim >= dataInicio.Value))
				.ToListAsync();

			var transacoesGeradas = new List<Transacao>();

			var dataCorrente = dataInicio.Value;
			while (dataCorrente <= dataFim.Value)
			{
				foreach (var regra in regrasRecorrentes)
				{
					if (dataCorrente >= regra.DataInicio && (regra.DataFim == null || dataCorrente <= regra.DataFim))
					{
						int diasNoMes = DateTime.DaysInMonth(dataCorrente.Year, dataCorrente.Month);
						if (regra.DiaDoMes <= diasNoMes)
						{
							var dataOcorrencia = new DateOnly(dataCorrente.Year, dataCorrente.Month, regra.DiaDoMes);
							if (dataOcorrencia >= dataInicio.Value && dataOcorrencia <= dataFim.Value)
							{
								transacoesGeradas.Add(new Transacao
								{
									Descricao = regra.Descricao,
									Valor = regra.Valor,
									Data = dataOcorrencia,
									Tipo = regra.Tipo,
									Categoria = regra.Categoria,
									CategoriaId = regra.CategoriaId
								});
							}
						}
					}
				}
				dataCorrente = dataCorrente.AddMonths(1);
			}
			var resultadoFinal = transacoesReais.Concat(transacoesGeradas)
										.OrderByDescending(t => t.Data)
										.ToList();

			return View(resultadoFinal);
		}
	}
}