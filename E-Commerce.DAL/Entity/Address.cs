using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity),Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter your StreetName")]
        public string? StreetName { get; set; }

        [Required(ErrorMessage = "Please enter your BuildingNo")]
        public string? BuildingNo { get; set; }
        public int AreaId { get; set; }
        public Area? Area { get; set; }
        public string? CountryCode { get; set; }
        public string? NearestLandmark { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
        public ICollection<Order>? Orders { get; set; } = new List<Order>();



    }
}
