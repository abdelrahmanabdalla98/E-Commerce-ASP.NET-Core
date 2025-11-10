using E_Commerce.BLL.Models.ApplicationModels;
using E_Commerce.DAL.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ProfileModels
{
    public enum GenderList
    {
        Male = 0,
        Female = 1,
        other = 2
    }
    public enum CountryCode
    {
        Egypt = +20,
        SaudiArabia = +966,
        UAE = +971,
        Jordan = +962
    }
    public class ProfileDTO
    {
        [Required]
        public string Id { get; set; }

        [PersonalData]
        public string? FirstName { get; set; }

        [PersonalData]
        public string? LastName { get; set; }
        public DateTime PasswordChanged { get; set; }
        public string? PassStrength { get; set; }
        public string? PassDictionaryJson { get; set; }
        public List<DateTime> LoginHistory { get; set; }

        public string? RecovEmail { get; set; }
        public string? RecovPhone { get; set; }
        public int? Gender { get; set; }
        public string? PhotoPath { get; set; }
        public List<string>? SavedPhotos { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        public IFormFile? PhotoFile { get; set; }
        public List<AddressDTO>? Addresses { get; set; }
    }
}
