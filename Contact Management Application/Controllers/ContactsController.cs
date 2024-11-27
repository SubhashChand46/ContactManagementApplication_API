using Contact_Management_Application.Models;
using Contact_Management_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Contact_Management_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly ContactJsonFile _jsonFileService;

        public ContactsController(ContactJsonFile jsonFileService)
        {
            _jsonFileService = jsonFileService;
        }

        [HttpGet("GetContacts")]
        public IActionResult GetContacts()
        {
            var contacts = _jsonFileService.GetAllContacts();
            return Ok(contacts);
        }

        [HttpPost("AddUpdateContact")]
        public IActionResult AddUpdateContact([FromBody] Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.FirstName) || string.IsNullOrWhiteSpace(contact.LastName) || string.IsNullOrWhiteSpace(contact.Email))
            {
                return BadRequest("All fields are required.");
            }
            var contacts = _jsonFileService.GetAllContacts();
            string returnMessage = string.Empty;
            if (contact.Id == 0)
            {
                contact.Id = contacts.Count > 0 ? contacts.Max(c => c.Id) + 1 : 1;
                contacts.Add(contact);
                returnMessage = "Data has been successfully saved";
            }
            else
            {
                var existingContact = contacts.FirstOrDefault(c => c.Id == contact.Id);
                if (existingContact != null)
                {
                    existingContact.FirstName = contact.FirstName;
                    existingContact.LastName = contact.LastName;
                    existingContact.Email = contact.Email;
                    returnMessage = "Data has been successfully updated";
                }
                else
                {
                    return NotFound("Contact not found.");
                }
            }
            _jsonFileService.SaveContacts(contacts);
            return Ok(new { contact, message = returnMessage, status = 1 });
        }

        [HttpDelete("DeleteContact")]
        public IActionResult DeleteContact(int id)
        {
            var contacts = _jsonFileService.GetAllContacts();
            var contact = contacts.FirstOrDefault(c => c.Id == id);
            if (contact == null) return NotFound();

            contacts.Remove(contact);
            _jsonFileService.SaveContacts(contacts);
            return Ok(new { message = "Data has been deleted successfully!", status = 1 });
        }
    }
}
