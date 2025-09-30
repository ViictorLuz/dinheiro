using Dinheiro.Data;
using Dinheiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace Dinheiro.Controllers
{
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var dataInicio = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
			var dataFim = dataInicio.AddMonths(1).AddDays(-1);

			var transacoesReais = await _context.Transacoes
				.Include(t => t.Categoria)
				.Where(t => t.Data >= dataInicio && t.Data <= dataFim)
				.ToListAsync();

			var regrasRecorrentes = await _context.TransacoesRecorrentes
				.Include(t => t.Categoria)
				.Where(r => r.Ativo && r.DataInicio <= dataFim && (r.DataFim == null || r.DataFim >= dataInicio))
				.ToListAsync();

			var transacoesGeradas = new List<Transacao>();
			foreach (var regra in regrasRecorrentes)
			{
				if (regra.DiaDoMes <= dataFim.Day)
				{
					transacoesGeradas.Add(new Transacao
					{
						Descricao = regra.Descricao,
						Valor = regra.Valor,
						Data = new DateOnly(dataInicio.Year, dataInicio.Month, regra.DiaDoMes),
						Tipo = regra.Tipo,
						Categoria = regra.Categoria,
						CategoriaId = regra.CategoriaId
					});
				}
			}

			var transacoesCombinadas = transacoesReais.Concat(transacoesGeradas).ToList();

			var totalReceitas = transacoesCombinadas.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
			var totalDespesas = transacoesCombinadas.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);
			var saldo = totalReceitas - totalDespesas;

			ViewBag.TotalReceitas = totalReceitas;
			ViewBag.TotalDespesas = totalDespesas;
			ViewBag.Saldo = saldo;

			var despesasPorCategoria = transacoesCombinadas
				.Where(t => t.Tipo == TipoTransacao.Despesa)
				.GroupBy(t => t.Categoria.Titulo)
				.Select(g => new { Categoria = g.Key, Total = g.Sum(t => t.Valor) })
				.OrderByDescending(d => d.Total)
				.ToList();

			ViewBag.LabelsDoGrafico = System.Text.Json.JsonSerializer.Serialize(despesasPorCategoria.Select(d => d.Categoria));
			ViewBag.ValoresDoGrafico = System.Text.Json.JsonSerializer.Serialize(despesasPorCategoria.Select(d => d.Total));

			var transacoesRecentes = await _context.Transacoes
				.Include(t => t.Categoria)
				.OrderByDescending(t => t.Data)
				.Take(5)
				.ToListAsync();

			return View(transacoesRecentes);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}