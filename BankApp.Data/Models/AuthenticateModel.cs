﻿using System.ComponentModel.DataAnnotations;

namespace BankApp.Data.Models
{
    public class AuthenticateModel
    {
        [Required]
        [RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{9}$")] // Account number must be 10
        public string AccountNumber { get; set; }
        [Required]
        public string Pin { get; set; }
    }
}
