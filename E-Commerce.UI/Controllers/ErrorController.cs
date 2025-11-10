using DocumentFormat.OpenXml.Wordprocessing;
using E_Commerce.DAL.Database;
using E_Commerce.DAL.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.UI.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ApplicationContext context;

        public ErrorController(ApplicationContext context)
        {
            this.context = context;
        }
        [HttpGet("/Error/RateLimit")]
        public IActionResult RateLimit()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Check()
        {

            var topLevel = context.Categories
               .Where(c => c.ParentCategoryId == null)
               .Include(c => c.SubCategories)
               .ThenInclude(sc => sc.SubCategories)
               .ToList();

            return View();
        }
    }
}