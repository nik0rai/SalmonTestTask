namespace ContactsSyncService.Models
{
    /// <summary>
    /// Table Contacts
    /// </summary>
    public class Contacts
    {
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Home { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? FullName { get; set; }
        public bool? Gender { get; set; }
    }
}
