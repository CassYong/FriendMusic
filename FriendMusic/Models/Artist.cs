using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FriendMusic.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Artist name is required")]
        [StringLength(200, ErrorMessage = "Artist name must not exceed 200 characters")]
        public string Name { get; set; }

        public string Biography { get; set; }

        public ICollection<Album> Albums { get; set; } // One-to-many relationship with albums
    }
}
