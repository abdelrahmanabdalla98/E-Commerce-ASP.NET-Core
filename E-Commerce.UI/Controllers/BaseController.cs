
//using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using E_Commerce.BLL.Models.ApplicationModels;
using E_Commerce.BLL.Repository;
using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Category = E_Commerce.DAL.Entity.Category;


namespace E_Commerce.PL.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private readonly ILogger<BaseController> _logger;
        private readonly IGenericRepository _Repo;
        private readonly UserManager<ApplicationUser> _manager;

        public BaseController(ILogger<BaseController> logger, IGenericRepository Repo,UserManager<ApplicationUser> _manager)
        {
            _logger = logger;
            _Repo = Repo;
            this._manager = _manager;
        }
        [AllowAnonymous]
        public IActionResult Home()
        {
            return View();
        }
        [HttpGet]

        public async Task<IActionResult> Index()
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




        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> AddToWL(string Obj)
        //{
        //    var model = JsonConvert.DeserializeObject<ShopDTO>(Obj)!;
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (userId != null)
        //    {
        //        var Available = await _Repo.GetAsync<WishlistDTO, WishList>(fil => fil.applicationUserId == userId, 
        //            true, opt => opt.Include(inc => inc.WishlistItems));

        //        if (Available == null)
        //        {
        //            Available = new WishlistDTO()
        //            {
        //                LastUpdated = DateTime.UtcNow,
        //                applicationUserId = userId,
        //                WishlistItems = new List<WishListItemDTO>()
        //            {
        //                new WishListItemDTO()
        //                {
        //                    CreatedOn = DateTime.UtcNow,
        //                    productId = model.WishListProduct,
        //                }
        //            }
        //            };
        //            var result = await _Repo.CreateAsync<WishlistDTO, WishList>(Available);
        //            model.WishListAdded = true;
        //            model.ProductsWL.Add(model.WishListProduct);
        //        }
        //        else
        //        {
        //            if (!Available.WishlistItems.Any(i => i.productId == model.WishListProduct))
        //            {
        //                var item = new WishListItemDTO()
        //                {
        //                    CreatedOn = DateTime.UtcNow,
        //                    productId = model.WishListProduct,
        //                    wishListId = Available.Id
        //                };
        //                var result = await _Repo.CreateAsync<WishListItemDTO, WishListItem>(item);
        //                model.WishListAdded = true;
        //                model.ProductsWL.Add(model.WishListProduct);
        //            }
        //            else
        //            {
        //                model.WishListAdded = false;

        //            }
        //        }
        //    }
        //    return View("Index",model);
        //}
        //[HttpPost]
        //public async Task<IActionResult> GetPage(string Obj)
        //{
        //    var model = JsonConvert.DeserializeObject<ShopDTO>(Obj)!;

        //    //apply sort criteria
        //    var sortAsc = model.SortType;

        //    if (sortAsc == 1)
        //    {
        //        model.Products = model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(null, 10, model.PageIndex,
        //    true, x => x.Include(inc => inc.category).ThenInclude(inc2 => inc2.ParentCategory)
        //        .ThenInclude(inc3 => inc3.ParentCategory).Include(inc2 => inc2.stock), sort => sort.ReviewCount);
        //    }
        //    else if (sortAsc == 2)
        //    {
        //        model.Products = model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(null, 10, model.PageIndex,
        //                        true, x => x.Include(inc => inc.category).ThenInclude(inc2 => inc2.ParentCategory)
        //        .ThenInclude(inc3 => inc3.ParentCategory).Include(inc2 => inc2.stock), sort => sort.ReviewRate);
        //    }
        //    else if (sortAsc == 3)
        //    {
        //        model.Products = model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(null, 10, model.PageIndex,
        //                        true, x => x.Include(inc => inc.category).ThenInclude(inc2 => inc2.ParentCategory)
        //        .ThenInclude(inc3 => inc3.ParentCategory).Include(inc2 => inc2.stock), sort => sort.CreatedOn);
        //    }
        //    else if (sortAsc == 4)
        //    {
        //        model.Products = model.Products = await _Repo.GetAllAsync<ProductDTO, Product>(null, 10, model.PageIndex,
        //                        true, x => x.Include(inc => inc.category).ThenInclude(inc2 => inc2.ParentCategory)
        //        .ThenInclude(inc3 => inc3.ParentCategory).Include(inc2 => inc2.stock), sort => sort.Price);
        //    }
        //    //handle here what should happen if values is null
        //    return View("Index", model);
        //}
        //[HttpPost]
        //public async Task<IActionResult> Filter(string Obj)
        //{
        //    var model = JsonConvert.DeserializeObject<ShopDTO>(Obj);

        //    var AllModels = await _Repo.GetAllAsync<ProductDTO,Product>(null, null, null, true,
        //        inc=>inc.Include(x=>x.category).Include(x2=>x2.stock));

        //    if (model.Filter != null)
        //    {
        //        if (model.Filter.Search != "")
        //        {
        //            //.Contains() allows partial matches (e.g., "lap" matches "laptop").

        //            //StringComparison.OrdinalIgnoreCase makes it case -insensitive.

        //                                AllModels = AllModels.Where(prod =>
        //                prod.Name.Contains(model.Filter.Search, StringComparison.OrdinalIgnoreCase) ||
        //                prod.category.Name.Contains(model.Filter.Search, StringComparison.OrdinalIgnoreCase)
        //            );

        //        }
        //        if (model.Filter.Categories.Count > 0)
        //        {
        //            AllModels = AllModels.Where(fil => model.Filter.Categories.Contains((int)fil.categoryId));
        //        }
        //        if (model.Filter.Sizes.Count>0)
        //        {
        //            AllModels = AllModels.Where(fil => model.Filter.Sizes.Contains(fil.stock.Size));
        //        }
        //        if (model.Filter.Colors.Count > 0)
        //        {
        //            AllModels = AllModels.Where(fil => model.Filter.Colors.Contains(fil.stock.Color));
        //        }
        //        model.Products = AllModels.Where(prod => prod.AfterDiscount > model.Filter.PriceMin && prod.AfterDiscount < model.Filter.PriceMax).ToList();
        //    }
        //    //var prods = await _Repo.GetAllAsync<ProductDTO, Product>(null
        //    //        , null, null, true, inc => inc.Include(x => x.category).Include(inc => inc.stock));
        //    //if (!model.categories.IsNullOrEmpty())
        //    //{


        //    //    prods = prods.Where((fil => model.categories.Contains(fil.category.Id)
        //    //        || model.categories.Contains((int)fil.category.ParentCategoryId)
        //    //        || model.categories.Contains((int)fil.category.ParentCategory.ParentCategoryId))).ToList();


        //    //}
        //    //if (!model.sizes.IsNullOrEmpty())
        //    //{
        //    //    prods = prods.Where(fil => model.sizes.Contains(fil.stock.Size)).ToList();
        //    //}
        //    //if (!model.colors.IsNullOrEmpty())
        //    //{
        //    //    prods = prods.Where(fil => model.colors.Contains(fil.stock.Color)).ToList();
        //    //}
        //    //prods = prods.Where(fil => (fil.AfterDiscount > model.priceMin && fil.AfterDiscount < model.priceMax)).ToList();
        //    //ViewBag.Filters = model;
        //    ////handle here what should happen if values is null
        //    //return View("Index", prods);
        //    return View("Index", model);

        //}
        public async Task<IActionResult> Index1()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var model = await _Repo.GetAsync<WishlistDTO, WishList>(fil => fil.applicationUserId == userId,true,
                opt=> opt.Include(inc=>inc.WishlistItems).ThenInclude(x=>x.product).ThenInclude(x2=>x2.stock));

            return View(model);
        }
        [HttpGet]
        public IActionResult Product_Details()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ProductDashboard(int PageSize=10,int PageIndex = 1)
        {
            ViewBag.PageSize = PageSize;
            ViewBag.PageIndex = PageIndex;
            ViewBag.ProdsSize = await _Repo.GetSize<Product>();
            ViewBag.cats = new SelectList(await _Repo.GetAllAsync<CategoryDTO, Category>(x => x.ParentCategory.ParentCategoryId == null),"Id","Name");
            var model = await _Repo.GetAllAsync<ProductDTO, Product>(null, PageSize, PageIndex, true, inc => inc.Include(x=>x.stock).Include(y=>y.category));
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CategoryDashboard()
        {
            ViewBag.x = TempData["category"];
            var model = await _Repo.GetAllAsync<CategoryDTO, Category>();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Decide(int Id,string action,string type)
        {
            // Set
            HttpContext.Session.SetInt32("id", Id);
            if (type == "Product")
            {
                var model = await _Repo.GetAsync<ProductDTO, Product>(x => x.Id == Id, true);
                if (action == "Delete")
                {
                    var ret = await _Repo.DeleteAsync<ProductDTO, Product>(model);
                    if (!ret)
                    {
                        _logger.LogInformation("Product {0} Not Deleted Successfully", model.Name);
                        return Json(new { ret = false });
                    }
                    return Json(new { ret = true });
                }
                else if (action == "Edit")
                {
                    return RedirectToAction("EditProduct");
                }
                return RedirectToAction("ProductDashboard");
            }
            else
            {
                var model = await _Repo.GetAsync<CategoryDTO, Category>(x => x.Id == Id, true);
                if (action == "Delete")
                {
                    var ret = await _Repo.DeleteAsync<CategoryDTO, Category>(model);
                    if (!ret)
                    {
                        _logger.LogInformation("Category {0} Not Deleted Successfully", model.Name);
                        return Json(new { ret = false });
                    }
                    return Json(new { ret = true });
                }
                else if (action == "Edit")
                {
                    return RedirectToAction("EditCategory");
                }
                else if (action== "AddSubCat")
                {
                    HttpContext.Session.SetString("action", action);

                    return RedirectToAction("AddCategory");
                }
                return RedirectToAction("ProductDashboard");
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditProduct()
        {
            var productId = HttpContext.Session.GetInt32("id");
            var model = await _Repo.GetAsync<ProductDTO, Product>(x => x.Id == productId,
                true,inc => inc.Include(x=>x.category).Include(inc=>inc.stock));

            
            ViewBag.Allstocks = new SelectList(await _Repo.GetAllAsync<StockDTO, Stock>(), "Id", "InventoryName",model.stock.Id);

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductDTO model)
        {
            if(model.Photos==null && model.PhotoPaths != null)
            {
                ModelState.ClearValidationState(nameof(model.Photos));
                ModelState.MarkFieldValid(nameof(model.Photos));
            }
            model.MainPhotoPath = model.PhotoPaths[0];
            if (ModelState.IsValid)
            {
                if (model.Photos != null)
                {
                    model.PhotoPaths!.Clear(); // always not null
                    foreach (var item in model.Photos)
                    {
                        if (item != null && item.Length > 0)
                        {
                            // Validate file type
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                            var fileExtension = Path.GetExtension(item.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                ModelState.AddModelError("PhotoFile", "Invalid file type. Only JPG, PNG, and GIF files are allowed.");
                                return View(model);
                            }
                            // Validate file size (e.g., max 5MB)
                            if (item.Length > 5 * 1024 * 1024)
                            {
                                ModelState.AddModelError("PhotoFile", "File size cannot exceed 5MB.");
                                return View(model);
                            }
                            // Generate unique filename
                            var fileName = Guid.NewGuid().ToString() + Path.GetFileName(item.FileName);
                            var folder = Directory.GetCurrentDirectory() + @"\wwwroot\uploads\photos\";


                            // Ensure directory exists
                            //Directory.CreateDirectory(uploadsFolder);

                            var filePath = Path.Combine(folder, fileName);

                            // Delete old photo if exists
                            if (model.SavedPhotos == null)
                            {
                                model.SavedPhotos = new List<string>();
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await item.CopyToAsync(fileStream);
                                }
                                model.SavedPhotos.Add(item.FileName + '|' + fileName);
                                model.PhotoPaths.Add("/uploads/photos/" + fileName);
                            }
                            else
                            {
                                bool chk = false;
                                for (int f = 0; f < model.SavedPhotos.Count; f++)
                                {
                                    if (model.SavedPhotos[f].Split('|')[0] == item.FileName)
                                    {
                                        model.PhotoPaths.Add("/uploads/photos/" + model.SavedPhotos[f].Split('|')[1]);
                                        chk = true; break;
                                    }
                                }
                                if (!chk)
                                {
                                    // Save file
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await item.CopyToAsync(fileStream);
                                    }
                                    model.SavedPhotos.Add(item.FileName + '|' + fileName);
                                    model.PhotoPaths.Add("/uploads/photos/" + fileName);
                                }
                            }
                        }

                    }

                }
                var rslt = await _Repo.UpdateAsync<ProductDTO, Product>(model);
                if (!rslt)
                {
                    _logger.LogInformation("Product {0} Not Updated Successfully", model.Name);
                }
                ViewBag.result = rslt;
            }
            _logger.LogInformation("Product {0} Not Updated Successfully", model.Name);
            ViewBag.stocks = new SelectList(await _Repo.GetAllAsync<StockDTO, Stock>(), "Id", "InventoryName");
            return View(model);
        }

        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult MyCart()
        {
            return View();
        }
        public IActionResult Profile2()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.stocks = new SelectList(await _Repo.GetAllAsync<StockDTO, Stock>(), "Id", "InventoryName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("AccountPolicy")]
        public async Task<IActionResult> AddProduct(ProductDTO model)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in model.Photos)
                {
                    if (item != null && item.Length > 0)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(item.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("PhotoFile", "Invalid file type. Only JPG, PNG, and GIF files are allowed.");
                            return View(model);
                        }
                        // Validate file size (e.g., max 5MB)
                        if (item.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("PhotoFile", "File size cannot exceed 5MB.");
                            return View(model);
                        }
                        // Generate unique filename
                        var fileName = Guid.NewGuid().ToString() + Path.GetFileName(item.FileName);
                        var folder = Directory.GetCurrentDirectory() + @"\wwwroot\uploads\photos\";


                        // Ensure directory exists
                        //Directory.CreateDirectory(uploadsFolder);

                        var filePath = Path.Combine(folder, fileName);

                        // Delete old photo if exists
                        if (model.SavedPhotos == null)
                        {
                            model.SavedPhotos = new List<string>();
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await item.CopyToAsync(fileStream);
                            }
                            model.SavedPhotos.Add(item.FileName + '|' + fileName);
                            model.PhotoPaths.Add("/uploads/photos/" + fileName);
                        }
                        else
                        {
                            bool chk = false;
                            for (int f = 0; f < model.SavedPhotos.Count; f++)
                            {
                                if (model.SavedPhotos[f].Split('|')[0] == item.FileName)
                                {
                                    model.PhotoPaths.Add("/uploads/photos/" + model.SavedPhotos[f].Split('|')[1]);
                                    chk = true; break;
                                }
                            }
                            if (!chk)
                            {
                                // Save file
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await item.CopyToAsync(fileStream);
                                }
                                model.SavedPhotos.Add(item.FileName + '|' + fileName);
                                model.PhotoPaths.Add("/uploads/photos/" + fileName);
                                
                            }
                        }
                    }

                }
                model.MainPhotoPath = model.PhotoPaths[0];
                var ret = await _Repo.CreateAsync<ProductDTO, Product>(model);
                if (ret)
                {
                    return RedirectToAction("Home");
                }
            }
            ViewBag.stocks = new SelectList(await _Repo.GetAllAsync<StockDTO, Stock>(), "Id", "InventoryName");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Categories(int Id)
        {
            if (Id == 0)
            {
                var cats = await _Repo.GetAllAsync<CategoryDTO, Category>(x => x.ParentCategoryId == null);
                return Json(new { ret = cats });
            }
            else if (Id == -1)
            {
                var cats = await _Repo.GetAllAsync<CategoryDTO, Category>(x => x.ParentCategory.ParentCategoryId == null);
                return Json(new { ret = cats });
            }
            else
            {
                var cats = await _Repo.GetAllAsync<CategoryDTO, Category>(x => x.ParentCategoryId == Id);
                return Json(new { ret = cats });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddCategory()
        {
            var categoryId = HttpContext.Session.GetInt32("id");
            var parentExist = HttpContext.Session.GetString("action");

            if (parentExist == "AddSubCat")
            {
                var model = await _Repo.GetAsync<CategoryDTO, Category>(x => x.Id == categoryId,true
                    ,inc=>inc.Include(x=>x.ParentCategory));
                if (model.ParentCategory == null)
                {
                    model.ParentCategory = new();
                    model.ParentCategoryId = model.Id;
                    model.ParentCategory.Name = model.Name;


                }
                if (model.ParentCategory.ParentCategoryId != null)
                {
                    ModelState.AddModelError("Name", "please select category max level 1");
                    TempData["category"] = "no";
                    return RedirectToAction("CategoryDashboard");
                }
                model.ParentCategory.Id = model.Id;
                model.ParentCategory.Name = model.Name;
                model.Name = "";
                ViewBag.category = parentExist;
                return View(model);
            }
            return View(new CategoryDTO());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddCategory(CategoryDTO model)
        {

            //if (model.ParentCategoryId == null)
            //{
            //    ModelState.ClearValidationState(nameof(model.ParentCategoryId));
            //    ModelState.MarkFieldValid(nameof(model.ParentCategoryId));
            //}
            if (ModelState.IsValid)
            {
                var check = await _Repo.CreateAsync<CategoryDTO, Category>(model);
                if (check)
                {
                    return RedirectToAction("Home");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory()
        {
            var categoryId = HttpContext.Session.GetInt32("id");
            var model = await _Repo.GetAsync<CategoryDTO, Category>(fil => fil.Id== categoryId, true);

            if (model.ParentCategoryId != null)
            {
                ViewBag.AllCategories = new SelectList(
                    await _Repo.GetAllAsync<CategoryDTO, Category>(fil => fil.ParentCategory.ParentCategoryId==null,null,null,true)
                    , "Id", "Name",model.ParentCategoryId);
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditCategory(CategoryDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var ret = await _Repo.UpdateAsync<CategoryDTO,Category>(model);
            if (!ret)
            {
                _logger.LogInformation("Category {0} Not Updated Successfully", model.Name);
            }
            ViewBag.success = ret;
            return View(model);
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult QuickView()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
