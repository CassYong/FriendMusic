using System;
using System.ComponentModel.DataAnnotations;

namespace FriendMusic.ViewModels
{
    public class AlbumEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Album title is required")]
        [StringLength(200, ErrorMessage = "Album title must not exceed 200 characters")]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Artist")]
        public int ArtistId { get; set; }

    }
}
