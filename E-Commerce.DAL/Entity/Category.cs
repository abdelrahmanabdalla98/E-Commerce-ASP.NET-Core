using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace E_Commerce.DAL.Entity
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    }

}
