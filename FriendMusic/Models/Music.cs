// Models/Music.cs
using FriendMusic.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;


namespace FriendMusic.Models
{
    public class Music
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        
        public string FilePath { get; set; } // Path where the music file is stored
        public DateTime UploadDate { get; set; }

        // New property to link music to the user
        [Required]
        public string UserId { get; set; }

        // Navigation property to the user
        public ApplicationUser User { get; set; }


    }
}
