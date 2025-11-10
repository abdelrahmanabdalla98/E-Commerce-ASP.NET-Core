using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity
{
    public class WishListItem
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public int productId { get; set; }
        public Product product { get; set; }
        public int wishListId { get; set; }
        public WishList wishList { get; set; }
    }
}
