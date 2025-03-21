// ViewModels/MusicUploadViewModel.cs
using Microsoft.AspNetCore.Http;

namespace FriendMusic.ViewModels
{
    public class MusicUploadViewModel
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }

        public IFormFile MusicFile { get; set; } // File to be uploaded
    }
}
