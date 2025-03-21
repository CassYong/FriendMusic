using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FriendMusic.ViewModels
{
    public class MusicEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; }

        [StringLength(200, ErrorMessage = "Artist must not exceed 200 characters")]
        public string Artist { get; set; }

        [StringLength(200, ErrorMessage = "Album must not exceed 200 characters")]
        public string Album { get; set; }
    }
}
