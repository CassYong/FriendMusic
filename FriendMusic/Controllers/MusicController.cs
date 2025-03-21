using FriendMusic.Data;
using FriendMusic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FriendMusic.ViewModels;
using Microsoft.AspNetCore.Identity;
using FriendMusic.Areas.Identity.Data;

namespace FriendMusic.Controllers
{
    [Authorize]
    public class MusicController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public MusicController(AuthDbContext context, IWebHostEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        // GET: /Music/Library
        public async Task<IActionResult> Library()
        {
            var userId = _userManager.GetUserId(User); 
            var musicList = await _context.Music
                                .Where(m => m.UserId == userId) 
                                .ToListAsync();
            return View(musicList);
        }

        // GET: /Music/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /Music/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(MusicUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "music");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.MusicFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.MusicFile.CopyToAsync(stream);
                }

                var userId = _userManager.GetUserId(User); 

                var music = new Music
                {
                    Title = model.Title,
                    Artist = model.Artist,
                    Album = model.Album,
                    FilePath = "/music/" + uniqueFileName, 
                    UploadDate = DateTime.Now,
                    UserId = userId
                };

                _context.Music.Add(music);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Library));
            }

            return View(model);
        }

        // GET: /Music/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User); 
            var music = await _context.Music
                                .Where(m => m.Id == id && m.UserId == userId) 
                                .FirstOrDefaultAsync();

            if (music == null)
            {
                return NotFound();
            }

            var editViewModel = new MusicEditViewModel
            {
                Id = music.Id,
                Title = music.Title,
                Artist = music.Artist,
                Album = music.Album 
            };

            return View(editViewModel);
        }

        // POST: /Music/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MusicEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User); 
                    var music = await _context.Music
                                        .Where(m => m.Id == id && m.UserId == userId) 
                                        .FirstOrDefaultAsync();

                    if (music == null)
                    {
                        return NotFound();
                    }

                    music.Title = model.Title;
                    music.Artist = model.Artist;
                    music.Album = model.Album; 

                    _context.Update(music);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Library));
            }

            return View(model);
        }


        // POST: /Music/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var music = await _context.Music.FindAsync(id);
            if (music == null)
            {
                return NotFound();
            }

            try
            {
                _context.Music.Remove(music);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Library));
        }



        private bool MusicExists(int id)
        {
            var userId = _userManager.GetUserId(User); 
            return _context.Music.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}

