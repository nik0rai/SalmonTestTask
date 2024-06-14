namespace ContactsSyncService.Models
{
    /// <summary>
    /// Payments table.
    /// </summary>
    public class Payments
    {
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? PaymentDetails { get; set; }
        public decimal? Ammount { get; set; }
        public DateTime? PaymentDateTime { get; set; }
    }
}
