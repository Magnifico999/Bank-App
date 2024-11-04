using BankApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Data.DTO
{
    public class TransactionRequest
    {
        [Key]
        public int Id { get; set; }
        public string TransactionUniqueReference { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public TranType TransactionType { get; set; }
        public string TransactionParticulars { get; set; }
        public TranStatus TransactionStatus { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
