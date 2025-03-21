namespace FriendMusic.ViewModels
{
    public class FriendRequestViewModel
    {
        public string RequesterId { get; set; }
        public string FriendId { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime RequestDate { get; set; }
        public string FriendName { get; set; }
    }
}
