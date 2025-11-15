using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothStore.Core.Entities
{
    public class Address : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Apartment { get; set; }
        public bool IsDefault { get; set; } = false;
        public string? PhoneNumber { get; set; }

        [ForeignKey("UserId")]
        public AspNetUser User { get; set; } = null!;

        [InverseProperty("Address")]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}


