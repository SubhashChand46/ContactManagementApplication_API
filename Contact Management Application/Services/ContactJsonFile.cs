using Contact_Management_Application.Models;
using System.Text.Json;

namespace Contact_Management_Application.Services
{
    public class ContactJsonFile
    {
        private readonly string _filePath;

        public ContactJsonFile(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.WebRootPath, "contacts.json");
        }

        public List<Contact> GetAllContacts()
        {
            if (!File.Exists(_filePath)) return new List<Contact>();
            var jsonData = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Contact>>(jsonData) ?? new List<Contact>();
        }

        public void SaveContacts(List<Contact> contacts)
        {
            var jsonData = JsonSerializer.Serialize(contacts);
            File.WriteAllText(_filePath, jsonData);
        }
    }
}
