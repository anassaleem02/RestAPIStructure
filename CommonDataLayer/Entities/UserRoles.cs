namespace CommonDataLayer.Entities
{
    public class UserRoles
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}
