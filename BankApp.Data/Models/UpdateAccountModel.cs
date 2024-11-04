using BankApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Data.Models
{
    public class UpdateAccountModel
    {

        [Key]
        public int Id { get; set; }
        [StringLength(11, ErrorMessage = "Phone number must not exceed 11 characters.")]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        public AccountType AccountType { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin must not be more than 4 digits")] 
        public string Pin { get; set; }
        [Required]
        [Compare("Pin", ErrorMessage = "Pins do not match")]
        public string ConfirmPin { get; set; }
        public DateTime DateLastUpdated { get; set; }
    }
}

