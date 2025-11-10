using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class ShopDTO
    {
        public int? PageIndex { get; set; } = 1;
        public int? ProductsSize { get; set; }
        public int SortType { get; set; } = 1;
        public string SortDir { get; set; } = "asc";
        public bool? WishListAdded { get; set; }
        public int WishListProduct { get; set; }
        public IList<int> ProductsWL { get; set; }= new List<int>();
        public IEnumerable<ProductDTO> Products { get; set; }
        public FilterOpts? Filter { get; set; }
        public List<StockItemDTO> AvailableColors { get; set; }
        public IEnumerable<CategoryDTO> AvailableCategories { get; set; }
        public List<StockItemDTO> AvailableSizes { get; set; }


    }
}
