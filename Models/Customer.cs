namespace FreshlyBackendNew.Models
{
    public class Customer
    {
        public Customer()
        {
            CustomerId = Guid.NewGuid(); // Auto-generate on instantiation
        }
        public Guid CustomerId { get; set; } // Changed from int to Guid
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string HouseNo { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
    }
}
