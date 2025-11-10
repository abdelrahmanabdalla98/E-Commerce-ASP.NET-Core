using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.Profile
{
    public class ValidationResultDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string SessionId { get; set; }

    }
}
