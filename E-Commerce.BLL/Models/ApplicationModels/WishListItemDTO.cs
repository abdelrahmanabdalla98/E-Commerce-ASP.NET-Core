using E_Commerce.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class WishListItemDTO
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public int productId { get; set; }
        public ProductDTO product { get; set; }
        public int wishListId { get; set; }
        public WishlistDTO wishList { get; set; }
    }
}
