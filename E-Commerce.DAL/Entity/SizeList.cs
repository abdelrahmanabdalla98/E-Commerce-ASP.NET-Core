using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity
{
    [Table("Sizes")]
    public class SizeList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
