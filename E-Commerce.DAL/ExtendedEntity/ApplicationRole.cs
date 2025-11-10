using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.ExtendedEntity
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsActive { get; set; }
    }
}
