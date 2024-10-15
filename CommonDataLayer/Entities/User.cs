using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommonDataLayer.Entities
{
    public class Users //: AuditModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore]
        [NotMapped]
        public int Full_Count { get; set; }
    }
}