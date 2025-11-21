using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.DAL.Entity_Extension;

namespace E_Commerce.DAL.Entity
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ReviewCount { get; set; }
        public int ReviewRate { get; set; }
        public bool FilterType { get; set; }
        public double Price { get; set; }
        public float Discount { get; set; }
        public int? AfterDiscount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<string>? PhotoPaths { get; set; }
        public List<string>? SavedPhotos { get; set; }
        public string MainPhotoPath { get; set; }
        public bool IsSale { get; set; }
        public int categoryId { get; set; }
        public Category category { get; set; }
        public List<Review> Reviews { get; set; } = new();
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public ICollection<StockItem> Items { get; set; }=new List<StockItem>();
    }

}
