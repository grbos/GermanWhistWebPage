﻿using System.ComponentModel.DataAnnotations;

namespace GermanWhistWebPage.Models
{
    public class UserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

        public int? PlayerId { get; set; }
    }
}
