﻿using System.ComponentModel.DataAnnotations;

namespace MiniTwit.Models
{
    public class UserCreateDTO
    {
        [Required]
        public string Username;

        [Required]
        public string Password;

        [Required]
        public string Email;
    }
}
