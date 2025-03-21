using System.ComponentModel.DataAnnotations;

namespace FriendMusic.ViewModels
{
    public class ArtistCreateViewModel
    {
        [Required(ErrorMessage = "Artist name is required")]
        [StringLength(200, ErrorMessage = "Artist name must not exceed 200 characters")]
        public string Name { get; set; }

        public string Biography { get; set; }
    }
}
