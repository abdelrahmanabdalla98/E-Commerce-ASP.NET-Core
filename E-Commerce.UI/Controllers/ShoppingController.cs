using DocumentFormat.OpenXml.Wordprocessing;
using E_Commerce.BLL.Models.ApplicationModels;
using E_Commerce.BLL.Repository;
using E_Commerce.DAL.Entity;
using E_Commerce.PL.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Security.Claims;

namespace E_Commerce.UI.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly ILogger<BaseController> logger;
        private readonly IGenericRepository _Repo;

        public ShoppingController(ILogger<BaseController> logger, IGenericRepository Repo)
        {
            this.logger = logger;
            _Repo = Repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]

        public async Task<IActionResult> Products()
        {
            var model = new ShopDTO();
            model.ProductsSize = await _Repo.GetSize<Product>();

            model.AvailableCategories = await _Repo.GetAllAsync<CategoryDTO, DAL.Entity.Category>(
                x => x.ParentCategoryId == null, null, null, true,
                x => x.Include(inc => inc.SubCategories).ThenInclude(sub => sub.SubCategories));


            model.AvailableSizes = (await _Repo.GetAllAsync<StockItemDTO, StockItem>()).DistinctBy(fil => fil.Size).ToList();

            model.AvailableColors = (await _Repo.GetAllAsync<StockItemDTO, StockItem>()).DistinctBy(_ => _.Color).ToList();


            model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(null, 10, model.PageIndex
                , true
                , x => x.Include(inc => inc.category).ThenInclude(inc2 => inc2.ParentCategory)
                .ThenInclude(inc3 => inc3.ParentCategory).Include(inc2 => inc2.stock)
                , x => x.ReviewCount);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var list = await _Repo.GetAllAsync<WishListItemDTO, WishListItem>(fil => fil.wishList.applicationUserId == userId, null, null, true);
            foreach (var item in list)
            {
                model.ProductsWL.Add(item.productId);
            }
            //var seenStockIds = new HashSet<string>(); // or int, depending on your StockId type
            //ViewBag.size = seenStockIds;
            //foreach (var product in model)
            //{
            //    if (product.stock != null && seenStockIds.Contains(product.stock.Size))
            //    {
            //        product.stock = null; // or skip rendering it
            //    }
            //    else
            //    {
            //        seenStockIds.Add(product.stock.Size);
            //    }
            //}
            // Now remove duplicates in stock based on Size

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetPage(string Obj)
        {
            var model = JsonConvert.DeserializeObject<ShopDTO>(Obj)!;

            //apply sort criteria
            var sortAsc = model.SortType;

            Expression<Func<Product,object>> sortExpression = sortAsc switch
            {
                1 => q => q.ReviewCount,
                2 => q => q.ReviewRate,
                3 => q => q.CreatedOn,
                4 => q => q.Price,
                _ => q => q.ReviewCount
            };

            model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(
                null, 10, model.PageIndex, true,
                ProductIncludes,
                sortExpression);

            //if (sortAsc == 1)
            //{
            //    model.Products = model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(null, 10, model.PageIndex,
            //true, x => x.Include(inc => inc.category).ThenInclude(inc2 => inc2.ParentCategory)
            //    .ThenInclude(inc3 => inc3.ParentCategory).Include(inc2 => inc2.stock), sort => sort.ReviewCount);
            //}
            //else if (sortAsc == 2)
            //{
            //    model.Products = model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(null, 10, model.PageIndex,
            //                    true, x => x.Include(inc => inc.category).ThenInclude(inc2 => inc2.ParentCategory)
            //    .ThenInclude(inc3 => inc3.ParentCategory).Include(inc2 => inc2.stock), sort => sort.ReviewRate);
            //}
            //else if (sortAsc == 3)
            //{
            //    model.Products = model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(null, 10, model.PageIndex,
            //                    true, x => x.Include(inc => inc.category).ThenInclude(inc2 => inc2.ParentCategory)
            //    .ThenInclude(inc3 => inc3.ParentCategory).Include(inc2 => inc2.stock), sort => sort.CreatedOn);
            //}
            //else if (sortAsc == 4)
            //{
            //    model.Products = model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(null, 10, model.PageIndex,
            //                    true, x => x.Include(inc => inc.category).ThenInclude(inc2 => inc2.ParentCategory)
            //    .ThenInclude(inc3 => inc3.ParentCategory).Include(inc2 => inc2.stock), sort => sort.Price);
            //}
            //handle here what should happen if values is null
            return View("Products", model);
        }
        private static Func<IQueryable<Product>, IQueryable<Product>> ProductIncludes =>
            x => x.Include(inc => inc.category)
          .ThenInclude(inc2 => inc2.ParentCategory)
          .ThenInclude(inc3 => inc3.ParentCategory)
          .Include(inc2 => inc2.stock);

        [HttpPost]
        public async Task<IActionResult> Filter(string Obj)
        {
            var model = JsonConvert.DeserializeObject<ShopDTO>(Obj);

            var AllModels = await _Repo.GetAllAsync<ProductDTO, Product>(null, null, null, true,
                ProductIncludes);

            if (model.Filter != null)
            {
                if (model.Filter.Search != "")
                {
                    //.Contains() allows partial matches (e.g., "lap" matches "laptop").

                    //StringComparison.OrdinalIgnoreCase makes it case -insensitive.

                    AllModels = AllModels.Where(prod =>
                    prod.Name.Contains(model.Filter.Search, StringComparison.OrdinalIgnoreCase) ||
                    prod.category.Name.Contains(model.Filter.Search, StringComparison.OrdinalIgnoreCase));
                }
                if (model.Filter.Categories.Count > 0)
                {
                    AllModels = AllModels.Where(fil => model.Filter.Categories.Contains((int)fil.categoryId)).ToList();
                }
                if (model.Filter.Sizes.Count > 0)
                {
                    AllModels = AllModels.Where(fil => model.Filter.Sizes.Contains(fil.stock.Size)).ToList();
                }
                if (model.Filter.Colors.Count > 0)
                {
                    AllModels = AllModels.Where(fil => model.Filter.Colors.Contains(fil.stock.Color)).ToList();
                }
                model.Products = AllModels.Where(prod => prod.AfterDiscount > model.Filter.PriceMin && prod.AfterDiscount < model.Filter.PriceMax).ToList();
            }
            //var prods = await _Repo.GetAllAsync<ProductDTO, Product>(null
            //        , null, null, true, inc => inc.Include(x => x.category).Include(inc => inc.stock));
            //if (!model.categories.IsNullOrEmpty())
            //{


            //    prods = prods.Where((fil => model.categories.Contains(fil.category.Id)
            //        || model.categories.Contains((int)fil.category.ParentCategoryId)
            //        || model.categories.Contains((int)fil.category.ParentCategory.ParentCategoryId))).ToList();


            //}
            //if (!model.sizes.IsNullOrEmpty())
            //{
            //    prods = prods.Where(fil => model.sizes.Contains(fil.stock.Size)).ToList();
            //}
            //if (!model.colors.IsNullOrEmpty())
            //{
            //    prods = prods.Where(fil => model.colors.Contains(fil.stock.Color)).ToList();
            //}
            //prods = prods.Where(fil => (fil.AfterDiscount > model.priceMin && fil.AfterDiscount < model.priceMax)).ToList();
            //ViewBag.Filters = model;
            ////handle here what should happen if values is null
            //return View("Index", prods);
            return View("Products", model);

        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWL(string Obj)
        {
            var model = JsonConvert.DeserializeObject<ShopDTO>(Obj)!;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var Available = await _Repo.GetAsync<WishlistDTO, WishList>(fil => fil.applicationUserId == userId,
                    true, opt => opt.Include(inc => inc.WishlistItems));

                if (Available == null)
                {
                    Available = new WishlistDTO()
                    {
                        LastUpdated = DateTime.UtcNow,
                        applicationUserId = userId,
                        WishlistItems = new List<WishListItemDTO>()
                    {
                        new WishListItemDTO()
                        {
                            CreatedOn = DateTime.UtcNow,
                            productId = model.WishListProduct,
                        }
                    }
                    };
                    var result = await _Repo.CreateAsync<WishlistDTO, WishList>(Available);
                    model.WishListAdded = true;
                    model.ProductsWL.Add(model.WishListProduct);
                }
                else
                {
                    WishListItemDTO Item;
                    if (!Available.WishlistItems.Any(i => i.productId == model.WishListProduct))
                    {
                        Item = new WishListItemDTO()
                        {
                            CreatedOn = DateTime.UtcNow,
                            productId = model.WishListProduct,
                            wishListId = Available.Id
                        };
                        var result = await _Repo.CreateAsync<WishListItemDTO, WishListItem>(Item);
                        model.WishListAdded = true;
                        model.ProductsWL.Add(model.WishListProduct);
                    }
                    else
                    {
                        Item = await _Repo.GetAsync<WishListItemDTO, WishListItem>(fil => fil.productId == model.WishListProduct, true);
                        var result = await _Repo.DeleteAsync<WishListItemDTO, WishListItem>(Item);
                        model.ProductsWL.Remove(model.WishListProduct);
                        model.WishListAdded = false;
                    }
                }
            }
            return View("Products", model);
        }
        [Authorize]
        public async Task<IActionResult> GetWisList()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var model = await _Repo.GetAsync<WishlistDTO, WishList>(fil => fil.applicationUserId == userId, true,
                opt => opt.Include(inc => inc.WishlistItems).ThenInclude(x => x.product).ThenInclude(x2 => x2.stock));

            return View(model);
        }


    }
}
