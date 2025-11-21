using E_Commerce.DAL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Internal;


namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class ProductDTO
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the name")]
        [StringLength(30, ErrorMessage = "Please enter a name with a maximum of 30 characters")]
        public string Name { get; set; }

        public int ReviewCount { get; set; }
        public int ReviewRate { get; set; }
        public bool FilterType { get; set; }

        [Required(ErrorMessage ="Please enter price")]
        [Range(1, 50000, ErrorMessage = "Max price is 50000")]
        public double? Price { get; set; }

        public float? Discount { get; set; }

        public double? AfterDiscount => Discount.HasValue
            ? Price * (1 - Discount.Value / 100)
            : Price;

        public string Currency { get; set; } = "EGP";

        [Required(ErrorMessage = "Please enter a description of the product")]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Please attach all photos of the product")]
        [MinLength(1, ErrorMessage = "At least one photo is required")]
        public List<IFormFile> Photos { get; set; }
        public List<string> PhotoPaths { get; set; } = new();
        public List<string>? SavedPhotos { get; set; }
        public string? MainPhotoPath { get; set; }
        public bool IsSale { get; set; }


        [Required(ErrorMessage = "Please select category")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select category in range")]
        public int? categoryId { get; set; }
        public CategoryDTO? category { get; set; }
        public StockItemDTO? Item { get; set; }
        public List<StockItemDTO>? Items { get; set; } = new(); 
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
    public class FilterOpts
    {
        public string? Search { get; set; }
        public IList<int> Categories { get; set; }
        public IList<string> Sizes { get; set; }
        public IList<string> Colors { get; set; }
        public int PriceMin { get; set; }
        public int PriceMax { get; set; }

    }
}
