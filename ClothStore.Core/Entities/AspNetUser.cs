using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothStore.Core.Entities
{
    public class AspNetUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string FullName =>
            !string.IsNullOrWhiteSpace($"{FirstName} {LastName}".Trim())
                ? $"{FirstName} {LastName}".Trim()
                : UserName ?? string.Empty;

        [InverseProperty("User")]
        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        [InverseProperty("User")]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}


