using System.Text.Json.Serialization;

namespace CommonDataLayer.DTOs
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore]
        public int Full_Count { get; set; }
    }
}
