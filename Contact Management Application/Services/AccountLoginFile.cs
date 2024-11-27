using Contact_Management_Application.Models;
using System.Text.Json;

namespace Contact_Management_Application.Services
{
    public class AccountLoginFile
    {
        private readonly string _filePath;

        public AccountLoginFile(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.WebRootPath, "Account.json");
        }

        public List<Account> GetAllAccounts()
        {
            if (!File.Exists(_filePath)) return new List<Account>();
            var jsonData = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Account>>(jsonData) ?? new List<Account>();
        }

        public void SaveAccounts(List<Account> accounts)
        {
            var jsonData = JsonSerializer.Serialize(accounts);
            File.WriteAllText(_filePath, jsonData);
        }
    }
}
