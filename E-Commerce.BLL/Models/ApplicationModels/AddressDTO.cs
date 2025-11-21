using E_Commerce.DAL.DB_Context;
using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class AddressDTO
    {
        public int Id { get; set; }

        [Required, MaxLength(100, ErrorMessage = "Please enter within 100 characters")]
        public string StreetName { get; set; }
        [Required]
        public string BuildingNo { get; set; }
        [Required]
        public int AreaId { get; set; }
        public Area Area { get; set; }

        [Required]
        public string CountryCode { get; set; }

        [MaxLength(200, ErrorMessage = "Please enter within 100 characters")]
        public string NearestLandmark { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public SelectList getCodes()
        {
            var countryCodes = new Dictionary<string, string>
                {
                    { "+20", "+20 (Egypt)" },
                    { "+966", "+966 (Saudi Arabia)" },
                    { "+971", "+971 (UAE)" },
                    { "+962", "+962 (Jordan)" }
                };

            var selectList = countryCodes
                .Select(code => new SelectListItem
                {
                    Value = code.Key,
                    Text = code.Value
                }).ToList();

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
