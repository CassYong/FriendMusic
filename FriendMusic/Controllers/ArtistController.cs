using FriendMusic.Data;
using FriendMusic.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FriendMusic.ViewModels;
using Microsoft.Extensions.Hosting.Internal;
using System.IO;
using System.Threading.Tasks;

namespace FriendMusic.Controllers
{
    public class ArtistController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ArtistController(AuthDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: /Artist
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 15)
        {
            // Calculate the number of items to skip
            int skip = (pageNumber - 1) * pageSize;

            // Retrieve artists with albums, paginated
            var artists = await _context.Artists
                .Include(a => a.Albums)
                .OrderBy(a => a.Name)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Total count of artists for pagination
            var totalArtists = await _context.Artists.CountAsync();
            var totalPages = (int)Math.Ceiling(totalArtists / (double)pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(artists);
        }

        // GET: /Artist/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Artist/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArtistCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var artist = new Artist
                {
                    Name = model.Name,
                    Biography = model.Biography
                };

                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        // GET: /Artist/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .Include(a => a.Albums)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // GET: /Artist/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            // Map the Artist entity to the ArtistEditViewModel
            var model = new ArtistEditViewModel
            {
                Id = artist.Id,
                Name = artist.Name,
                Biography = artist.Biography
            };

            return View(model);
        }

        // POST: /Artist/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArtistEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Retrieve the existing artist from the database
            var existingArtist = await _context.Artists.FindAsync(id);
            if (existingArtist == null)
            {
                return NotFound();
            }

            // Update properties of the existing artist
            existingArtist.Name = model.Name;
            existingArtist.Biography = model.Biography;

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating artist: {ex.Message}");
                throw;
            }
        }


        // GET: /Artist/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .FirstOrDefaultAsync(m => m.Id == id);

            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: /Artist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
}
