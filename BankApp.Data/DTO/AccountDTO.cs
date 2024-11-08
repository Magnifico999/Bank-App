﻿using BankApp.Data.Enums;
using BankApp.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Data.DTO
{
    public class AccountDTO
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CurrentAccountBalance { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountNumberGenerated { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

            }
}
