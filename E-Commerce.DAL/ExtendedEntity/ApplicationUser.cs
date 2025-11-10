using E_Commerce.DAL.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity_Extension
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? FirstName { get; set; }

        [PersonalData]
        public string? LastName { get; set; }

        public int? Gender { get; set; }
        public string? ProfilePhoto { get; set; }
        public List<string>? SavedPhotos { get; set; }

        public DateOnly Birthday { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime PasswordChanged { get; set; }
        public string? PassStrength { get; set; }
        public string? PassDictionaryJson { get; set; }
        public string? RecovEmail { get; set; }
        public string? RecovPhone { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<DateTime> LoginHistory { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<PaymentCard> Cards { get; set; } = new List<PaymentCard>();
    }
}
