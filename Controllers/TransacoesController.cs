using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dinheiro.Data;
using Dinheiro.Models;

namespace Dinheiro.Controllers
{
	public class TransacoesController : Controller
	{
		private readonly ApplicationDbContext _context;

		public TransacoesController(ApplicationDbContext context)
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

			var transacoes = await _context.Transacoes
				.Include(t => t.Categoria)
				.Include(t => t.Conta)
				.Where(t => t.Data >= dataInicio.Value && t.Data <= dataFim.Value)
				.OrderByDescending(t => t.Data)
				.ToListAsync();

			return View(transacoes);
		}

		// GET: Transacoes/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transacao = await _context.Transacoes
				.Include(t => t.Categoria)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (transacao == null)
			{
				return NotFound();
			}

			return View(transacao);
		}

		// GET: Transacoes/Create
		public IActionResult Create()
		{
			ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo");
			ViewData["ContaId"] = new SelectList(_context.Contas, "Id", "Nome");
			return View();
		}

		// POST: Transacoes/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(
	[Bind("Id,Descricao,Valor,Data,Tipo,CategoriaId,ContaId")] Transacao transacao,
	bool EhParcelado, int NumeroDeParcelas)
		{
			ModelState.Remove("Conta");
			ModelState.Remove("Categoria");

			if (ModelState.IsValid)
			{
				if (EhParcelado && NumeroDeParcelas > 1)
				{
					for (int i = 0; i < NumeroDeParcelas; i++)
					{
						var transacaoParcelada = new Transacao
						{
							Descricao = $"{transacao.Descricao} ({i + 1}/{NumeroDeParcelas})",
							Valor = transacao.Valor,
							Data = transacao.Data.AddMonths(i),
							Tipo = transacao.Tipo,
							CategoriaId = transacao.CategoriaId,
							ContaId = transacao.ContaId
						};
						_context.Add(transacaoParcelada);
					}
					TempData["success"] = $"{NumeroDeParcelas} parcelas criadas com sucesso!";
				}
				else
				{
					_context.Add(transacao);
					TempData["success"] = "Transação criada com sucesso!";
				}

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo", transacao.CategoriaId);
			ViewData["ContaId"] = new SelectList(_context.Contas, "Id", "Nome", transacao.ContaId);
			return View(transacao);
		}

		// GET: Transacoes/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transacao = await _context.Transacoes.FindAsync(id);
			if (transacao == null)
			{
				return NotFound();
			}
			ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo", transacao.CategoriaId);
			ViewData["ContaId"] = new SelectList(_context.Contas, "Id", "Nome", transacao.ContaId);
			return View(transacao);
		}

		// POST: Transacoes/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,Valor,Data,Tipo,CategoriaId,ContaId")] Transacao transacao)
		{
			if (id != transacao.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(transacao);
					await _context.SaveChangesAsync();
					TempData["success"] = "Transação atualizada com sucesso!";
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TransacaoExists(transacao.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo", transacao.CategoriaId);
			return View(transacao);
		}

		// GET: Transacoes/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transacao = await _context.Transacoes
				.Include(t => t.Categoria)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (transacao == null)
			{
				return NotFound();
			}

			return View(transacao);
		}

		// POST: Transacoes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var transacao = await _context.Transacoes.FindAsync(id);
			if (transacao != null)
			{
				_context.Transacoes.Remove(transacao);
			}

			await _context.SaveChangesAsync();
			TempData["success"] = "Transação deletada com sucesso!";
			return RedirectToAction(nameof(Index));
		}

		private bool TransacaoExists(int id)
		{
			return _context.Transacoes.Any(e => e.Id == id);
		}
	}
}
