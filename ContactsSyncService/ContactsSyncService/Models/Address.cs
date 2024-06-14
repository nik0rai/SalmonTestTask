namespace ContactsSyncService.Models
{
    /// <summary>
    /// Address table.
    /// </summary>
    public class Address
    {
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Home { get; set; }
        public bool? IsActive { get; set; }
        public bool? AddresType { get; set; }
    }
}
