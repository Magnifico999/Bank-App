using System.Net;

namespace BankApp.Data.DTO
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>(); 
        }
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public List<string> ErrorMessages { get; set; }

        public object Result { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
