using E_Commerce.DAL.Entity_Extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity
{
    public class WishList
    {
        public int Id { get; set; }
        public ICollection<WishListItem> WishlistItems { get; set; } = new List<WishListItem>();
        public DateTime? LastUpdated { get; set; }
        public string applicationUserId { get; set; }
        public ApplicationUser applicationUser { get; set; }

    }
}
