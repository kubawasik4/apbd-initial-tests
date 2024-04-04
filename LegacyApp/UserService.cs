using System;

namespace LegacyApp
{
    public class UserService
    {
        private IClientRepository _clientRepository;
        private ICreditLimitService _creditLimitService;
        [Obsolete]
        public UserService()
        {
            _clientRepository = new ClientRepository();
            _creditLimitService = new UserCreditService();
        }

        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!CheckUserData(firstName, lastName, email)) return false;
            if (!CheckAge(dateOfBirth)) return false;
            
            var client = _clientRepository.GetById(clientId);
            var user = new User().CreateUser(client, dateOfBirth, email, firstName, lastName);
            
            user.SetCreditLimit(_creditLimitService, client);
            if (!user.ValidationCreditLimit()) return false;

            UserDataAccess.AddUser(user);
            return true;
        }
        public bool CheckAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;
            if (age < 21) return false;
            return true;
        }
        public bool CheckUserData(string firstName, string LastName, string email)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(LastName)) return false;
            if (!email.Contains("@") || !email.Contains(".")) return false;
            return true;
        }
    }
}
