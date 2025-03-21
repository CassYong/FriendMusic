using FriendMusic.Data;
using FriendMusic.Models;
using FriendMusic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FriendMusic.Controllers
{
    public class AlbumController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AlbumController(AuthDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Album
        public IActionResult Index(int pageNumber = 1, int pageSize = 16)
        {
            var albums = _context.Albums
                .OrderByDescending(a => a.ReleaseDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalAlbums = _context.Albums.Count();
            var totalPages = (int)Math.Ceiling(totalAlbums / (double)pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(albums);
        }


        // GET: /Album/Create
        public IActionResult Create()
        {
            PopulateArtistsDropDownList(); // Populate artists dropdown
            return View();
        }

        // POST: /Album/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlbumCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var album = new Album
                    {
                        Title = viewModel.Title,
                        ReleaseDate = viewModel.ReleaseDate,
                        ArtistId = viewModel.ArtistId
                    };

                    // Upload album art file
                    album.AlbumArtUrl = await UploadAlbumArtFile(viewModel.AlbumArt);

                    _context.Add(album);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving album: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the album.");
                }
            }

            PopulateArtistsDropDownList();
            return View(viewModel);
        }


        // GET: /Album/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            // Map Album to AlbumEditViewModel
            var viewModel = new AlbumEditViewModel
            {
                Id = album.Id,
                Title = album.Title,
                ReleaseDate = album.ReleaseDate,
                ArtistId = album.ArtistId
            };

            PopulateArtistsDropDownList(); // Populate artists dropdown
            return View(viewModel);
        }

        // POST: /Album/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AlbumEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var album = await _context.Albums.FindAsync(id);
                    if (album == null)
                    {
                        return NotFound();
                    }

                    // Update properties from view model to album entity
                    album.Title = viewModel.Title;
                    album.ReleaseDate = viewModel.ReleaseDate;
                    album.ArtistId = viewModel.ArtistId;

                    _context.Update(album);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(viewModel.Id))
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
                    Console.WriteLine($"Error editing album: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while editing the album.");
                }
            }

            PopulateArtistsDropDownList();
            return View(viewModel);
        }


        // GET: /Album/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: /Album/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private void PopulateArtistsDropDownList()
        {
            var artists = _context.Artists.Select(a => new { a.Id, a.Name }).ToList();
            if (artists.Any())
            {
                Console.WriteLine($"Artists available for selection: {string.Join(", ", artists.Select(a => a.Name))}");
            }
            ViewBag.Artists = new SelectList(artists, "Id", "Name");
        }


        // Check if album exists
        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }

        private async Task<string> UploadAlbumArtFile(IFormFile albumArtFile)
        {
            if (albumArtFile != null && albumArtFile.Length > 0)
            {
                var fileExtension = Path.GetExtension(albumArtFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException("Invalid file type. Please upload a file of type: jpg, jpeg, png, webp.");
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "albums");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(albumArtFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await albumArtFile.CopyToAsync(stream);
                }

                return "/albums/" + uniqueFileName; 
            }

            return null; 
        }


    }
}
