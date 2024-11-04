using BankApp.Data.Models;

namespace BankApp.Views
{
    public class SearchResultsViewModel
    {
        public List<GetAccountModel> AccountSearchResults { get; set; }
        public List<Transaction> TransactionSearchResults { get; set; }
    }
}
