using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class WishlistDTO
    {
        public int Id { get; set; }
        public List<WishListItemDTO> WishlistItems { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string applicationUserId { get; set; }
        public ApplicationUser applicationUser { get; set; }

    }
}
