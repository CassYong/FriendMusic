using FriendMusic.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace FriendMusic.Models
{
    public class Friendship
    {
        public int Id { get; set; }

        [Required]
        public string RequesterId { get; set; } // User sending the request

        [Required]
        public string FriendId { get; set; } // User receiving the request

        public bool? IsAccepted { get; set; }

        public DateTime RequestDate { get; set; }

        // Navigation properties
        [ForeignKey("RequesterId")]
        public ApplicationUser Requester { get; set; }

        [ForeignKey("FriendId")]
        public ApplicationUser Friend { get; set; }
    }
}
