using System.ComponentModel.DataAnnotations;

namespace FriendMusic.ViewModels
{
    public class FriendshipCreateViewModel
    {
        [Required(ErrorMessage = "Friend's Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string FriendId { get; set; }
    }
}
