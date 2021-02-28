using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Infrastructure;
using Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Core.Interfaces;

namespace Web.Controllers
{
    public class PortfolioItemsController : Controller
    {
        private readonly IUnitOfWork<PortfolioItem> _portfolio;
        private readonly IHostingEnvironment _hosting;

        public PortfolioItemsController(IUnitOfWork<PortfolioItem> portfolio, IHostingEnvironment hosting)
        {
            _portfolio = portfolio;
            _hosting = hosting;
        }

        // GET: PortfolioItems
        public IActionResult Index()
        {
            return View(_portfolio.Entity.GetAll());
        }

        // GET: PortfolioItems/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolioItem = _portfolio.Entity.GetById(id);
            if (portfolioItem == null)
            {
                return NotFound();
            }

            return View(portfolioItem);
        }

        // GET: PortfolioItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PortfolioItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PortfolioViewModels models)
        {
            if (ModelState.IsValid)
            {
                if (models.File != null)
                {
                    string upload = Path.Combine(_hosting.WebRootPath, @"img\portfolio\");
                    string fullPath = Path.Combine(upload, models.File.FileName);
                    models.File.CopyTo(new FileStream(fullPath, FileMode.Create));
                }
                PortfolioItem portfolioItem = new PortfolioItem
                {
                    ProjectName = models.ProjectName,
                    Description = models.Description,
                    ImageUrl = models.File.FileName
                };
                _portfolio.Entity.Insert(portfolioItem);
                _portfolio.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(models);
        }

        // GET: PortfolioItems/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var portfolioItem = _portfolio.Entity.GetById(id);
            if (portfolioItem == null)
            {
                return NotFound();
            }
            PortfolioViewModels portfolio = new PortfolioViewModels
            {
                Id = portfolioItem.Id,
                Description = portfolioItem.Description,
                ProjectName = portfolioItem.ProjectName,
                ImageUrl = portfolioItem.ImageUrl
            };
            return View(portfolio);
        }

        // POST: PortfolioItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, PortfolioViewModels models)
        {
            if (id != models.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (models.File != null)
                    {
                        string upload = Path.Combine(_hosting.WebRootPath, @"img\portfolio\");
                        string fullPath = Path.Combine(upload, models.File.FileName);
                        models.File.CopyTo(new FileStream(fullPath, FileMode.Create));
                    }
                    PortfolioItem portfolioItem = new PortfolioItem
                    {
                        Id = models.Id,
                        ProjectName = models.ProjectName,
                        Description = models.Description,
                        ImageUrl = models.File.FileName
                    };
                    _portfolio.Entity.Update(portfolioItem);
                    _portfolio.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortfolioItemExists(models.Id))
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
            return View(models);
        }

        // GET: PortfolioItems/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolioItem = _portfolio.Entity.GetById(id);
            if (portfolioItem == null)
            {
                return NotFound();
            }

            return View(portfolioItem);
        }

        // POST: PortfolioItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _portfolio.Entity.Delete(id);
            _portfolio.Save();
            return RedirectToAction(nameof(Index));
        }

        private bool PortfolioItemExists(Guid id)
        {
            return _portfolio.Entity.GetAll().Any(e => e.Id == id);
        }
    }
}
