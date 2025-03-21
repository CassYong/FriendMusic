// SendMessageViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace FriendMusic.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        public string ReceiverId { get; set; }

        public string ReceiverName { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Content { get; set; }
    }
}
