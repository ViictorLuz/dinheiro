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
	public class TransacoesRecorrentesController : Controller
	{
		private readonly ApplicationDbContext _context;

		public TransacoesRecorrentesController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: TransacoesRecorrentes
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.TransacoesRecorrentes.Include(t => t.Categoria);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: TransacoesRecorrentes/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transacaoRecorrente = await _context.TransacoesRecorrentes
				.Include(t => t.Categoria)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (transacaoRecorrente == null)
			{
				return NotFound();
			}

			return View(transacaoRecorrente);
		}

		// GET: TransacoesRecorrentes/Create
		public IActionResult Create()
		{
			ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo");
			return View();
		}

		// POST: TransacoesRecorrentes/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Descricao,Valor,Tipo,DiaDoMes,DataInicio,DataFim,Ativo,CategoriaId")] TransacaoRecorrente transacaoRecorrente)
		{
			if (ModelState.IsValid)
			{
				_context.Add(transacaoRecorrente);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo", transacaoRecorrente.CategoriaId);
			return View(transacaoRecorrente);
		}

		// GET: TransacoesRecorrentes/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transacaoRecorrente = await _context.TransacoesRecorrentes.FindAsync(id);
			if (transacaoRecorrente == null)
			{
				return NotFound();
			}
			ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo", transacaoRecorrente.CategoriaId);
			return View(transacaoRecorrente);
		}

		// POST: TransacoesRecorrentes/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,Valor,Tipo,DiaDoMes,DataInicio,DataFim,Ativo,CategoriaId")] TransacaoRecorrente transacaoRecorrente)
		{
			if (id != transacaoRecorrente.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(transacaoRecorrente);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TransacaoRecorrenteExists(transacaoRecorrente.Id))
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
			ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo", transacaoRecorrente.CategoriaId);
			return View(transacaoRecorrente);
		}

		// GET: TransacoesRecorrentes/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transacaoRecorrente = await _context.TransacoesRecorrentes
				.Include(t => t.Categoria)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (transacaoRecorrente == null)
			{
				return NotFound();
			}

			return View(transacaoRecorrente);
		}

		// POST: TransacoesRecorrentes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var transacaoRecorrente = await _context.TransacoesRecorrentes.FindAsync(id);
			if (transacaoRecorrente != null)
			{
				_context.TransacoesRecorrentes.Remove(transacaoRecorrente);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool TransacaoRecorrenteExists(int id)
		{
			return _context.TransacoesRecorrentes.Any(e => e.Id == id);
		}
	}
}
