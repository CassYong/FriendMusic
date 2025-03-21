using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FriendMusic.Data;
using FriendMusic.Models;
using System.Security.Claims;
using FriendMusic.ViewModels;

namespace FriendMusic.Controllers
{
    [Authorize]
    public class SocialController : Controller
    {
        private readonly AuthDbContext _context;

        public SocialController(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            var friendships = await _context.Friendship
                .Include(f => f.Requester)
                .Include(f => f.Friend)
                .Where(f => f.RequesterId == userId || f.FriendId == userId)
                .ToListAsync();

            ViewData["CurrentUserId"] = userId; 

            return View(friendships);
        }

        // GET: Social/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Social/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FriendshipCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

                var friend = await _context.Users.FirstOrDefaultAsync(u => u.Email == viewModel.FriendId);
                if (friend == null)
                {
                    ModelState.AddModelError("FriendId", "User with this email does not exist.");
                    return View(viewModel);
                }

                if (friend.Id == userId)
                {
                    ModelState.AddModelError("FriendId", "You cannot send a friend request to yourself.");
                    return View(viewModel);
                }

                var existingRequest = await _context.Friendship.FirstOrDefaultAsync(f =>
                    (f.RequesterId == userId && f.FriendId == friend.Id) ||
                    (f.RequesterId == friend.Id && f.FriendId == userId));

                if (existingRequest != null)
                {
                    ModelState.AddModelError("FriendId", "A friend request has already been sent to this user or exists in pending state.");
                    return View(viewModel);
                }

                var friendship = new Friendship
                {
                    RequesterId = userId,
                    FriendId = friend.Id,
                    RequestDate = DateTime.Now,
                    IsAccepted = null
                };

                _context.Add(friendship);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Social/Respond/5
        public async Task<IActionResult> Respond(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendship = await _context.Friendship
                .Include(f => f.Requester)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (friendship == null)
            {
                return NotFound();
            }

            var viewModel = new FriendRequestViewModel
            {
                RequesterId = friendship.RequesterId,
                FriendId = friendship.FriendId,
                IsAccepted = friendship.IsAccepted ?? false,
                RequestDate = friendship.RequestDate,
                FriendName = friendship.Requester != null ? friendship.Requester.UserName : "Unknown"
            };

            return View(viewModel); 
        }

        // POST: Social/Respond/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(int id, bool? accept)
        {
            var friendship = await _context.Friendship.FindAsync(id);
            if (friendship == null)
            {
                return NotFound();
            }

            if (accept.HasValue && accept.Value)
            {
                
                friendship.IsAccepted = true;
            }
            else
            {
                
                friendship.IsAccepted = false; 
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // POST: Social/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var friendship = await _context.Friendship.FindAsync(id);
            if (friendship == null)
            {
                return NotFound();
            }

            _context.Friendship.Remove(friendship);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Social/Accept/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var friendship = await _context.Friendship.FindAsync(id);
            if (friendship == null)
            {
                return NotFound();
            }

            friendship.IsAccepted = true; 
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Social/SendMessage
        public async Task<IActionResult> SendMessage(string friendId)
        {
            if (string.IsNullOrEmpty(friendId))
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var friendship = await _context.Friendship
                .Include(f => f.Requester)
                .Include(f => f.Friend)
                .FirstOrDefaultAsync(f =>
                    (f.RequesterId == currentUserId && f.FriendId == friendId && f.IsAccepted == true) ||
                    (f.RequesterId == friendId && f.FriendId == currentUserId && f.IsAccepted == true));

            if (friendship == null)
            {
                return NotFound();
            }

            var viewModel = new SendMessageViewModel
            {
                ReceiverId = friendId,
                ReceiverName = friendship.RequesterId == currentUserId ? friendship.Friend.UserName : friendship.Requester.UserName
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(SendMessageViewModel viewModel, string friendId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }

                var friendship = await _context.Friendship
                    .Include(f => f.Requester)
                    .Include(f => f.Friend)
                    .FirstOrDefaultAsync(f =>
                        (f.RequesterId == currentUserId && f.FriendId == friendId && f.IsAccepted == true) ||
                        (f.RequesterId == friendId && f.FriendId == currentUserId && f.IsAccepted == true));

                if (friendship != null)
                {
                    viewModel.ReceiverName = friendship.RequesterId == currentUserId ? friendship.Friend.UserName : friendship.Requester.UserName;
                }

                return View(viewModel);
            }

            // Create and save the message
            var message = new Message
            {
                SenderId = currentUserId,
                ReceiverId = viewModel.ReceiverId,
                Content = viewModel.Content,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewMessages", new { friendId = viewModel.ReceiverId });

        }

        // GET: Social/ViewMessages
        public async Task<IActionResult> ViewMessages(string friendId)
        {
            if (string.IsNullOrEmpty(friendId))
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            // Check if they are friends (accepted)
            var friendship = await _context.Friendship.FirstOrDefaultAsync(f =>
                (f.RequesterId == currentUserId && f.FriendId == friendId && f.IsAccepted == true) ||
                (f.RequesterId == friendId && f.FriendId == currentUserId && f.IsAccepted == true));

            if (friendship == null)
            {
                return NotFound();
            }

            var messages = await _context.Messages
                .Include(m => m.Sender) 
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == friendId) || (m.SenderId == friendId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return View(messages);
        }

        // GET: Social/EditMessage/5
        public async Task<IActionResult> EditMessage(int? messageId)
        {
            if (messageId == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(messageId);
            if (message == null || message.SenderId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            // Map the Message entity to EditMessageViewModel
            var viewModel = new EditMessageViewModel
            {
                Id = message.Id,
                Content = message.Content,
                ReceiverId = message.ReceiverId
            };

            return View(viewModel);
        }


        // POST: Social/EditMessage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMessage(EditMessageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Retrieve the existing message from the database
            var existingMessage = await _context.Messages.FindAsync(viewModel.Id);
            if (existingMessage == null || existingMessage.SenderId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            // Update the message content
            existingMessage.Content = viewModel.Content;

            _context.Update(existingMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewMessages", new { friendId = existingMessage.ReceiverId });
        }


        // POST: Social/DeleteMessage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null || message.SenderId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewMessages", new { friendId = message.ReceiverId });
        }

        private bool FriendshipExists(int id)
        {
            return _context.Friendship.Any(e => e.Id == id);
        }
    }
}
