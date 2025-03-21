using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FriendMusic.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Album title is required")]
        [StringLength(200, ErrorMessage = "Album title must not exceed 200 characters")]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public int ArtistId { get; set; } // Foreign key to Artist table
        public Artist Artist { get; set; } // Navigation property

        // New property for album picture
        public string AlbumArtUrl { get; set; }
    }
}
