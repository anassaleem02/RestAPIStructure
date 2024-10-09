namespace CommonDataLayer.Entities
{
    public class AuditModel
    {
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
