using E_Commerce.BLL.Models.ApplicationModels;
using E_Commerce.BLL.Repository;
using E_Commerce.DAL.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace E_Commerce.UI.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly IGenericRepository _Repo;
        private readonly ILogger<StockItemDTO> logger;

        public StockController(IGenericRepository _Repo , ILogger<StockItemDTO> logger)
        {
            this._Repo = _Repo;
            this.logger = logger;
        }
        public async Task<IActionResult> StockManagement(int PageSize = 10, int PageIndex = 1)
        {
            ViewBag.PageSize = PageSize;
            ViewBag.PageIndex = PageIndex;
            ViewBag.ProdsSize = await _Repo.GetSize<StockItem>();
            var model = await _Repo.GetAllAsync<StockItemDTO, StockItem>(null, PageSize, PageIndex, true,
                inc => inc.Include(x => x.Stock).Include(y => y.Product));

            return View(model);
        }
        public IActionResult AddStockItem(){return View();}
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStockItem(StockItemDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                model.CreatedBy = User.Identity?.Name;
                var ok = await _Repo.CreateAsync<StockItemDTO, StockItem>(model);
                if (!ok)
                {
                    ViewBag.error = new string[] { "Error adding item", "check database for error","error" };
                    return View(model);
                }
                TempData["error"] = new string[] { "Item added", "Item added succesfully","success" };
                return RedirectToAction("StockManagement");

            }
            catch (Exception ex)
            {
                logger.LogInformation("Item {0} Not Added Successfully as Error {1}", model.Id,ex.Message);
                ViewBag.error = new string[]{"Error adding item", "check log file as there is an error" };
                return View(model);

            }
            //return RedirectToAction("StockManagement");
        }


        public async Task<IActionResult> UpdateStockItem(int id)
        {
            var Obj = await _Repo.GetAsync<StockItemDTO, StockItem>(par => par.Id == id, true,inc=>inc.Include(x=>x.Stock).Include(y=>y.Product));
            return View(Obj); 
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateStockItem(StockItemDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                model.CreatedBy = User.Identity?.Name;
                model.Stock.Id = model.StockId;
                var ok = await _Repo.UpdateAsync<StockItemDTO, StockItem>(model);
                if (!ok)
                {
                    ViewBag.error = new string[] { "Error updating item", "check database for error","error" };
                    return View(model);
                }
                TempData["error"] = new string[] { "Item Updated", "Item Updated Successfully", "success" };
                return RedirectToAction("StockManagement");

            }
            catch (Exception ex)
            {
                logger.LogInformation("Item {0} Not updated Successfully as Error {1}", model.Id, ex.Message);
                ViewBag.error = new string[] { "Error updating item", "check log file as there is an error", "error" };
                return View(model);

            }
            //return RedirectToAction("StockManagement");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var model = await _Repo.GetAsync<StockItemDTO, StockItem>(fil => fil.Id == id, true);
            try
            {
                var ok = await _Repo.DeleteAsync<StockItemDTO, StockItem>(model);
                return Json(new { res = ok });
            }
            catch (Exception ex)
            {
                logger.LogInformation("Item {0} Not Deleted Successfully as Error {1}", model.Id, ex.Message);
                ViewBag.error = new string[] { "Item Not Deleted", "check log file as there is an exception","error"};
                return Json(new { res = "exception"});

            }
            //return RedirectToAction("StockManagement");
        }

        public IActionResult AddStock() { return View(); }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStock(StockDTO model) 
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                model.CreatedOn = DateTime.Now;
                var ok = await _Repo.CreateAsync<StockDTO, Stock>(model);
                if (!ok)
                {
                    ViewBag.error = new string[] { "Stock Not Added", "Stock not added check database", "error" };
                    return View(model);
                }
                TempData["error"] = new string[] { "Stock Added", "Stock added successfully", "success" };
                return RedirectToAction("StockManagement");
            } catch (Exception ex)
            {
                logger.LogInformation("Stock {0} Not Added Successfully as Error {1}", model.Id, ex.Message);
                ViewBag.error = new string[] { "Error updating item", "check log file as there is an error", "error" };
                return View(model);
            }
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddSize(SizeList model)
        {
            try
            {
                var ok = await _Repo.CreateAsync<SizeList, SizeList>(model);
                if (!ok)
                {
                    return Json(new
                    {
                        res = false,
                        error = new string[] { "Size Not Added", "Size not added check database", "error" }
                    });
                }
                return Json(new
                {
                    res = true,
                    error = new string[] { "Stock Added", "Stock added successfully", "success" }
                });
            }
            catch (Exception ex)
            {
                logger.LogInformation("Size {0} Not Added Successfully as Error {1}", model.Name, ex.Message);
                return Json(new
                {
                    res = true,
                    error = new string[] { "Error updating item", "check log file as there is an error", "error" }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetSelectList(string[] type, int? id)
        {
            var Lists = new List<dynamic>();
            foreach (var item in type)
            {
                if (item == "product")
                {
                    Lists.Add((await _Repo.GetAllAsync<ProductDTO, Product>(null, null, null, true)).Select(x => new
                    {
                        value = x.Id, 
                        text = x.Name
                    }));

                }
                else if (item == "stock")
                {
                    Lists.Add((await _Repo.GetAllAsync<StockDTO, Stock>(null, null, null, true)).Select(x => new
                    {
                        value = x.Id,
                        text = x.InventoryName
                    }));
                }
                else if (item == "size")
                {
                    Lists.Add((await _Repo.GetAllAsync<SizeList, SizeList>(null, null, null, true)).Select(x => new
                    {
                        value = x.Id,
                        text = x.Name
                    }));
                }
            }
            return Json(new { allLists = Lists });
        }
    }
}
