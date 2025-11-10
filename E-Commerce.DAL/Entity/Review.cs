using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity
{
    public class Review
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }
        public string? Comment { get; set; }
        public float StarsCount { get; set; }
        public int LikesCount { get; set; }
        public ICollection<Product> Reviews { get; set; } = new List<Product>();
        public string ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

    }
}
