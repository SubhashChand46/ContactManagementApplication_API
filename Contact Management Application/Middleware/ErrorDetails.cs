using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Contact_Management_Application.Middleware
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; } 
        public string Message { get; set; }
        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}
