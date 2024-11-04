using BankApp.Data.DTO;

namespace BankApp.Core.Services.Interface
{
    public interface IAuthService
    {
        Task<ResponseDto<string>> ExternalLogin(string email, string firstName, string surname);
       
    }
}
