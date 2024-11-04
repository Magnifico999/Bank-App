using System.ComponentModel.DataAnnotations;

namespace BankApp.Data.DTO
{
    public class MakeTransfer
    {
        [Required(ErrorMessage = "Account Number is required.")]
        [RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{9}$", ErrorMessage = "Account Number must be 10-digit")]
        public string FromAccount { get; set; }
        [Required(ErrorMessage = "Account Number is required.")]
        [RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{9}$", ErrorMessage = "Account Number must be 10-digit")]
        public string ToAccount { get; set; }

        //[Required(AllowEmptyStrings =true)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Transaction Pin is required.")]
        public string TransactionPin { get; set; }
    }
}
