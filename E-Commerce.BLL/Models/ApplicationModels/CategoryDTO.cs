using E_Commerce.DAL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please enter Category Name"), MaxLength(30, ErrorMessage = "max length 30 characters")]
        public string Name { get; set; }
        //[Required(ErrorMessage ="Please select category")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select correct Category")]
        public int? ParentCategoryId { get; set; }
        public CategoryDTO? ParentCategory { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public List<CategoryDTO>? SubCategories { get; set; }
    }
}
