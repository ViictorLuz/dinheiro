using Dinheiro.Data;
using Dinheiro.Models;
using Dinheiro.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Dinheiro.Controllers
{
	public class ProjecaoController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ProjecaoController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(decimal? valorSimulacao, DateOnly? dataSimulacao, int numeroParcelas = 1)
		{
			var saldoInicialContas = await _context.Contas.SumAsync(c => c.SaldoInicial);
			var totalReceitasReais = await _context.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).SumAsync(t => t.Valor);
			var totalDespesasReais = await _context.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).SumAsync(t => t.Valor);
			var saldoAtualReal = saldoInicialContas + totalReceitasReais - totalDespesasReais;

			var tresMesesAtras = DateOnly.FromDateTime(DateTime.Now.AddMonths(-3));
			var despesasVariaveisPassadas = await _context.Transacoes
				.Where(t => t.Tipo == TipoTransacao.Despesa && t.Data >= tresMesesAtras && t.Data < DateOnly.FromDateTime(DateTime.Now))
				.ToListAsync();
			var mediaGastosVariaveisMensal = despesasVariaveisPassadas.Any() ? despesasVariaveisPassadas.Sum(t => t.Valor) / 3m : 0;


			var projecao = new List<ProjecaoMensalViewModel>();
			var saldoInicialProjecao = saldoAtualReal;
			var dataAtual = DateTime.Now;

			var valorParcela = 0m;
			if (valorSimulacao.HasValue && numeroParcelas > 0)
			{
				valorParcela = valorSimulacao.Value / numeroParcelas;
			}

			for (int i = 0; i < 12; i++)
			{
				var mesProjecao = dataAtual.AddMonths(i);
				var dataInicioMes = new DateOnly(mesProjecao.Year, mesProjecao.Month, 1);
				var dataFimMes = dataInicioMes.AddMonths(1).AddDays(-1);

				var recorrenciasDoMes = await _context.TransacoesRecorrentes
					.Where(r => r.Ativo && r.DataInicio <= dataFimMes && (r.DataFim == null || r.DataFim >= dataInicioMes))
					.ToListAsync();

				var receitasRecorrentes = recorrenciasDoMes.Where(r => r.Tipo == TipoTransacao.Receita).Sum(r => r.Valor);
				var despesasRecorrentes = recorrenciasDoMes.Where(r => r.Tipo == TipoTransacao.Despesa).Sum(r => r.Valor);

				var despesaSimuladaEsteMes = 0m;
				if (valorSimulacao.HasValue && dataSimulacao.HasValue)
				{
					var dataFimSimulacao = dataSimulacao.Value.AddMonths(numeroParcelas - 1);
					if (dataInicioMes <= dataFimSimulacao && dataFimMes >= dataSimulacao.Value)
					{
						despesaSimuladaEsteMes = valorParcela;
					}
				}

				var totalDespesasProjetadas = despesasRecorrentes + mediaGastosVariaveisMensal + despesaSimuladaEsteMes;
				var saldoFinalMes = saldoInicialProjecao + receitasRecorrentes - totalDespesasProjetadas;

				projecao.Add(new ProjecaoMensalViewModel
				{
					MesAno = dataInicioMes.ToString("MMMM/yyyy", new CultureInfo("pt-BR")),
					SaldoInicial = saldoInicialProjecao,
					TotalReceitas = receitasRecorrentes,
					TotalDespesas = totalDespesasProjetadas,
					DespesasSimuladas = despesaSimuladaEsteMes,
					SaldoFinal = saldoFinalMes
				});

				saldoInicialProjecao = saldoFinalMes;
			}

			ViewData["valorSimulacao"] = valorSimulacao;
			ViewData["dataSimulacao"] = dataSimulacao?.ToString("yyyy-MM-dd");
			ViewData["numeroParcelas"] = numeroParcelas > 1 ? numeroParcelas : 1; // Garante que o valor seja pelo menos 1

			return View(projecao);
		}
	}
}
