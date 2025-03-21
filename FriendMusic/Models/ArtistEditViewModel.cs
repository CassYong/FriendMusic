using System.ComponentModel.DataAnnotations;

namespace FriendMusic.ViewModels // Use the correct namespace here
{
    public class ArtistEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Artist name is required")]
        [StringLength(200, ErrorMessage = "Artist name must not exceed 200 characters")]
        public string Name { get; set; }

        public string Biography { get; set; }
    }
}

