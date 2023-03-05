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
using Perfumeshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Perfumeshop.Controllers
{
    public class UsersController : Controller
    {
        private readonly PerfumeshopContext _context;
        private readonly UserManager<PerfumeshopUser> _userManager;
        private readonly SignInManager<PerfumeshopUser> _signInManager;
        public UsersController(PerfumeshopContext context, UserManager<PerfumeshopUser> userManager, SignInManager<PerfumeshopUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
              return _context.User != null ? 
                          View(await _context.User.ToListAsync()) :
                          Problem("Entity set 'PerfumeshopContext.User'  is null.");
        }

        // GET: Users/Details/5
      //  [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }
            var pom = _context.Users.Where(x => x.user_ID == id).FirstOrDefault();
            if (pom != null)
            {
                ViewBag.Email = pom.Email;
            }
            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            UserEditPicture vm = new UserEditPicture
            {
                user = user,
                pictureName = user.ProfilePicture
            };

            return View(vm);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,ProfilePicture,user")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            UserEditPicture vm = new UserEditPicture
            {
                user = user,
                pictureName = user.ProfilePicture
            };
            return View(vm);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int id, UserEditPicture vm)
        {
            if (id != vm.user.Id)
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
                        vm.user.ProfilePicture = uniqueFileName;
                    }
                    else
                    {
                        vm.user.ProfilePicture = vm.pictureName;
                    }
                    _context.Update(vm.user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(vm.user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new {id = id});
            }
            return View(vm);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var pom = _context.Users.Where(x => x.user_ID == id).FirstOrDefault();
            if (pom != null)
            {
                ViewBag.Email = pom.Email;
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'PerfumeshopContext.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(int id)
        {
          return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> OrderMorePerfumes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _context.User.Where(x => x.Id == id).Include(x => x.Perfumes).First();
            if (user == null)
            {
                return NotFound();
            }
            var perfumes = _context.Perfume.AsEnumerable();
            perfumes = perfumes.OrderBy(s => s.Name);
            MorePerfumesVM vm = new MorePerfumesVM
            {
                
                user = user,
                perfumeList = new MultiSelectList(perfumes, "Id", "Name"),
                selectedPerfumes = user.Perfumes.Select(x => x.PerfumeId)
            };
            ViewBag.Message = vm.user.FullName;
            return View(vm);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> OrderMorePerfumes(int id, MorePerfumesVM vm)
        {
            if (id != vm.user.Id)
            {
                return NotFound();
            }


                
                   // _context.Update(vm.user);
                    //await _context.SaveChangesAsync();
                    IEnumerable<int> listPerfumes = vm.selectedPerfumes;
                        IQueryable<Order> toBeRemoved = _context.Order.Where(s => !listPerfumes.Contains(s.PerfumeId) && s.UserId == id);
                        _context.Order.RemoveRange(toBeRemoved);
                        IEnumerable<int> exist = _context.Order.Where(x => listPerfumes.Contains(x.PerfumeId) && x.UserId == id).Select(x => x.PerfumeId);
                        IEnumerable<int> newEn = listPerfumes.Where(x => !exist.Contains(x));

                        foreach (int perfume in newEn)
                            _context.Add(new Order { Status="Pending Approval", PerfumeId = perfume, UserId = id, Location=vm.location });

            await _context.SaveChangesAsync();
            return RedirectToAction("Orders","Orders", new {id=id});
        }
        [Authorize(Roles = "User")]
        private string UploadedFile(UserEditPicture vm)
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
        public async Task<IActionResult> CreateAcc(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View();
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAcc(int id, [Bind("Id,FirstName,LastName,ProfilePicture,user")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","Home");
            }
            return View(user);
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> OrderAPerfume(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfume =await  _context.Perfume.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (perfume == null)
            {
                return NotFound();
            }
           // var perfume = _context.Perfume.Where(x => x.Id == PerfumeId).First();
            var userIdentity = _userManager.GetUserAsync(User).Result.user_ID;
            var user = await _context.User.Where(x => x.Id == userIdentity).FirstOrDefaultAsync();
            PerfumeOrder vm = new PerfumeOrder
            {
                user = user,
                perfumeId = perfume.Id
            };
            ViewBag.User = vm.user.FullName;
            ViewBag.Perfume = perfume.Name;
            ViewBag.Price = perfume.Price;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> OrderAPerfume(int id, PerfumeOrder vm)
        {

            _context.Add(new Order { Status = "Pending Approval", PerfumeId =id, UserId = vm.user.Id, Location = vm.location });
            await _context.SaveChangesAsync();
            return RedirectToAction("Orders", "Orders", new { id = vm.user.Id });
        }
    }
}

