using BankApp.Data;
using BankApp.Data.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BankApp.Data.DTO;

namespace BankApp.Core.Repository.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;

        public AccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public Account Authenticate(string AccountNumber, string Pin)
        {
            var account = _db.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if (account == null || !VerifyPinHash(Pin, account.PinHash, account.PinSalt))
            {
                return null;
            }

            return account;
        }

        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentNullException("Pin");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for (int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<Response> Create(Account account, string Pin, string ConfirmPin)
        {
            var response = new Response();

            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                var existingAccountWithEmail = _db.Accounts.FirstOrDefault(x => x.Email == account.Email && x.Id != account.Id);
                if (existingAccountWithEmail != null)
                {
                    response.ResponseCode = "EmailExists";
                    response.ResponseMessage = "Email already exists in the database.";
                    response.Data = null;
                    return response; 
                }
            }

            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                var existingAccountWithPhoneNumber = _db.Accounts.FirstOrDefault(x => x.PhoneNumber == account.PhoneNumber && x.Id != account.Id);
                if (existingAccountWithPhoneNumber != null)
                {
                    response.ResponseCode = "PhoneNumberExists";
                    response.ResponseMessage = "Phone Number already exists in the database.";
                    response.Data = null;
                    return response; 
                }
            }

            if (!Pin.Equals(ConfirmPin))
            {
                response.ResponseCode = "PinsDoNotMatch";
                response.ResponseMessage = "Pins do not match.";
                response.Data = null;
                return response;
            }

            // Hashing/encrypting pin
            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt);

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(); 

            response.ResponseCode = "Success";
            response.ResponseMessage = "Account created successfully.";
            response.Data = account; 
            return response; 
        }


        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        public async Task Delete(int Id)
        {
            var response = new Response();
            var account =  _db.Accounts.Find(Id);
            if (account == null)
            {
                response.ResponseCode = "400";
                response.ResponseMessage = "There was a problem deleting an account";
                response.Data = null;

            }
            _db.Remove(account);
            await _db.SaveChangesAsync();

        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _db.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _db.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).FirstOrDefault();
            if (account == null)
            {
                throw new ArgumentException("Account number is not correct.");
            }

            return account;
        }

        public async Task<Account> GetById(int Id)
        {
            var account = await _db.Accounts.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (account == null) return null;

            return account;
        }
        public async Task<Response> Update(Account account, string Pin = null)
        {
            var response = new Response();
            var accountToBeUpdated = _db.Accounts.Find(account.Id);
            if (accountToBeUpdated == null)
            {
                throw new ApplicationException("Account does not exist");
            }
            

            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                var existingAccountWithEmail = _db.Accounts.FirstOrDefault(x => x.Email == account.Email && x.Id != account.Id);
                if (existingAccountWithEmail != null)
                {
                    response.ResponseCode = "EmailExists";
                    response.ResponseMessage = "Email already exists in the database.";
                    response.Data = null;
                    return response;
                }

                accountToBeUpdated.Email = account.Email;
            }

            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                var existingAccountWithPhoneNumber = _db.Accounts.FirstOrDefault(x => x.PhoneNumber == account.PhoneNumber && x.Id != account.Id);
                if (existingAccountWithPhoneNumber != null)
                {
                    response.ResponseCode = "PhoneNumberExist";
                    response.ResponseMessage = "Phone number already exists in the database.";
                    response.Data = null;
                    return response;
                }

                accountToBeUpdated.PhoneNumber = account.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(Pin))
            {
                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);
                accountToBeUpdated.PinHash = pinHash;
                accountToBeUpdated.PinSalt = pinSalt;
            }

            accountToBeUpdated.DateLastUpdated = DateTime.UtcNow;
            accountToBeUpdated.FirstName = account.FirstName;
            accountToBeUpdated.LastName = account.LastName;
            accountToBeUpdated.AccountType = account.AccountType;

            _db.Accounts.Update(accountToBeUpdated);
            await _db.SaveChangesAsync();
            return response;
        }


    }
}
