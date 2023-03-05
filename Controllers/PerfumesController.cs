using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Perfumeshop.Data;
using Perfumeshop.Models;
using Perfumeshop.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Perfumeshop.Controllers
{
    public class PerfumesController : Controller
    {
        private readonly PerfumeshopContext _context;

        public PerfumesController(PerfumeshopContext context)
        {
            _context = context;
        }

        // GET: Perfumes
      //  [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index(string perfumeCategory, string? perfumeBrand ,string searchString)
        {
            IQueryable<Perfume> perfumes = _context.Perfume.AsQueryable();
            IQueryable<string> categoryQuery = _context.Perfume.OrderBy(m => m.Category).Select(m => m.Category).Distinct();
            IQueryable<string> brandQuery = _context.Brand.OrderBy(m => m.Id).Select(m => m.Name).Distinct();
          
            if (!string.IsNullOrEmpty(searchString))
            {
                perfumes = perfumes.Where(s => s.Name.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(perfumeCategory))
            {
                perfumes = perfumes.Where(x => x.Category == perfumeCategory);
            }
            if (!string.IsNullOrEmpty(perfumeBrand))
            {
                string name = perfumeBrand;
                var brand = _context.Brand.Where(x => x.Name == name).First();
                perfumes = perfumes.Where(x => x.Brand.Id == brand.Id);
            }
            perfumes = perfumes.Include(m => m.Users).ThenInclude(m => m.User);
            var vm = new PerfumesFilterVM
            {
                Categories = new SelectList(await categoryQuery.ToListAsync()),
                Brands = new SelectList(await brandQuery.ToListAsync()),
                perfumes = await perfumes.ToListAsync()
            };
            return View(vm);
        }

        // GET: Perfumes/Details/5
       // [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Perfume == null)
            {
                return NotFound();
            }

            var perfume = await _context.Perfume
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (perfume == null)
            {
                return NotFound();
            }

            return View(perfume);
        }

        // GET: Perfumes/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brand, "Id", "Name");
            return View();
        }

        // POST: Perfumes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Category,Size,Price,Description,Picture,BrandId")] Perfume perfume)
        {
            if (ModelState.IsValid)
            {
                _context.Add(perfume);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brand, "Id", "Name", perfume.BrandId);
            return View(perfume);
        }

        // GET: Perfumes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Perfume == null)
            {
                return NotFound();
            }

            var perfume = await _context.Perfume.FindAsync(id);
            if (perfume == null)
            {
                return NotFound();
            }
            PerfumePicture vm = new PerfumePicture
            {
                perfume = perfume,
                pictureName = perfume.Picture
            };
            ViewData["BrandId"] = new SelectList(_context.Brand, "Id", "Name", perfume.BrandId);
            return View(vm);
        }

        // POST: Perfume/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, PerfumePicture vm)
        {
            if (id != vm.perfume.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (vm.pictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(vm);
                        vm.perfume.Picture = uniqueFileName;
                    }
                    else
                    {
                        vm.perfume.Picture = vm.pictureName;
                    }
                    _context.Update(vm.perfume);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerfumeExists(vm.perfume.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { id = vm.perfume.Id }) ;
            }
            ViewData["BrandId"] = new SelectList(_context.Brand, "Id", "Name", vm.perfume.BrandId);
            return View(vm);
        }

        // GET: Perfumes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Perfume == null)
            {
                return NotFound();
            }

            var perfume = await _context.Perfume
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (perfume == null)
            {
                return NotFound();
            }

            return View(perfume);
        }

        // POST: Perfumes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Perfume == null)
            {
                return Problem("Entity set 'PerfumeshopContext.Perfume'  is null.");
            }
            var perfume = await _context.Perfume.FindAsync(id);
            if (perfume != null)
            {
                _context.Perfume.Remove(perfume);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        private bool PerfumeExists(int id)
        {
          return (_context.Perfume?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [Authorize(Roles = "Admin")]
        private string UploadedFile(PerfumePicture vm)
        {
            string uniqueFileName = null;

            if (vm.pictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(vm.pictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    vm.pictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> BrandsPerfumes(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = await _context.Brand.FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Brand = brand.Name;
            IQueryable<Perfume> perfumes = _context.Perfume.Where(x => x.BrandId == id);
            await _context.SaveChangesAsync();
            return View(await perfumes.ToListAsync());
        }
    }
}
