using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        [Required, MaxLength(100, ErrorMessage = "max length is 100 characters")]
        public string Comment { get; set; }
        public float StarsCount { get; set; }
        public int LikesCount { get; set; }
        public ICollection<Product> Reviews { get; set; } = new List<Product>();
        public string applicationUserId { get; set; }
        public required ApplicationUser applicationUser { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
