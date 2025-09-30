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
    public class MetasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MetasController(ApplicationDbContext context)
        {
            _context = context;
        }

		// GET: Metas
		public async Task<IActionResult> Index()
		{
			return View(await _context.Metas.Include(m => m.Aportes).ToListAsync());
		}

		// GET: Metas/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var meta = await _context.Metas
				.Include(m => m.Aportes) // <-- ADICIONE ESTA LINHA PARA CARREGAR OS APORTES
				.FirstOrDefaultAsync(m => m.Id == id);

			if (meta == null)
			{
				return NotFound();
			}

			return View(meta);
		}

		// GET: Metas/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Metas/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Titulo,ValorAlvo,DataFinalPrevista")] Meta meta)
		{
			if (ModelState.IsValid)
			{
				_context.Add(meta);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(meta);
		}

		// GET: Metas/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var meta = await _context.Metas.FindAsync(id);
			if (meta == null)
			{
				return NotFound();
			}
			return View(meta);
		}

		// POST: Metas/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,ValorAlvo,DataFinalPrevista")] Meta meta)
		{
			if (id != meta.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(meta);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!MetaExists(meta.Id))
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
			return View(meta);
		}

		// GET: Metas/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var meta = await _context.Metas
				.FirstOrDefaultAsync(m => m.Id == id);
			if (meta == null)
			{
				return NotFound();
			}

			return View(meta);
		}

		// POST: Metas/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var meta = await _context.Metas.FindAsync(id);
			if (meta != null)
			{
				_context.Metas.Remove(meta);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool MetaExists(int id)
		{
			return _context.Metas.Any(e => e.Id == id);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AdicionarAporte([Bind("MetaId,Valor,Data")] Aporte aporte)
		{
			if (ModelState.IsValid)
			{
				_context.Aportes.Add(aporte);
				await _context.SaveChangesAsync();
				TempData["success"] = "Aporte adicionado com sucesso!";
			}
			else
			{
				TempData["error"] = "Ocorreu um erro ao adicionar o aporte.";
			}
			return RedirectToAction("Details", new { id = aporte.MetaId });
		}
	}
}
