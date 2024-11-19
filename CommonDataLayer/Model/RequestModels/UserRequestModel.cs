
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CommonDataLayer.Model.RequestModels
{
    public class UserRegistrationRequestModel
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int RoleId { get; set; }

        [Required]
        public string Email { get; set; }
        [JsonIgnore]
        public string? Salt { get; set; }
    }
    public class LoginRequestModel 
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
