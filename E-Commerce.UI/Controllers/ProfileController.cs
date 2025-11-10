using ClosedXML.Excel;
using E_Commerce.BLL.Models.ApplicationModels;
using E_Commerce.BLL.Models.ProfileModels;
using E_Commerce.BLL.Repository;
using E_Commerce.BLL.Services;
using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;


namespace E_Commerce.PL.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IOtpService _otpService;
        private readonly IGenericRepository _Repo;
        private readonly IAddressService _service;
        private readonly UserManager<ApplicationUser> userManager;
        private string? Id;
        public ProfileController(IOtpService otpService ,
            IGenericRepository _Repo, IAddressService _service,
            UserManager<ApplicationUser> userManager)
        {
            _otpService = otpService;
            this._Repo = _Repo;
            this._service = _service;
            this.userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            base.OnActionExecuting(context);
        }
        public async Task<IActionResult> PersonalInfo()
        {
            // TWO WAYS IS TO GET IT IN VIEWBAG OR ATTACH IT TO ALL CLASSES AAS PROPERTY PROFILE HEAD CLASS

            ViewBag.ActiveTab = "PersonalInfo";
            var model = await _Repo.GetAsync<ProfileDTO,ApplicationUser>(x => x.Id == Id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> PersonalInfo(ProfileDTO model)
        {
            // TWO WAYS IS TO GET IT IN VIEWBAG OR ATTACH IT TO ALL CLASSES AAS PROPERTY PROFILE HEAD CLASS
            var ckeck = await _Repo.UpdateAsync<ProfileDTO, ApplicationUser>(model);
            return RedirectToAction("PersonalInfo");
        }
        public async Task<IActionResult> Address()
        {
            ViewBag.ActiveTab = "Address";
            var model = await _Repo.GetAsync<ProfileDTO,ApplicationUser>(x => x.Id == Id,true,inc => inc.Include(add => add.Addresses!.Where(x => x.IsDeleted==false))
            .ThenInclude(are => are.Area)
            .ThenInclude(cit => cit!.City)
            .ThenInclude(cou => cou!.Country));
            return View(model);
        }
        [HttpPost]
        public IActionResult HandleOps(ProfileDTO model, string action)
        {
            TempData["ProfileJson"] = JsonConvert.SerializeObject(model);
            switch (action)
            {
                case "Edit":
                    // Use AddressIndex or v.Id as needed
                    return RedirectToAction("UpdateAddress");
                case "Delete":
                    return RedirectToAction("DeleteAddress");
                // Use AddressIndex or v.Id as needed
                case "Default":
                    return RedirectToAction("DefaultAddress");
                    // Use AddressIndex or v.Id as needed
            }
            return View();
        }
        public async Task<IActionResult> AddAddress()
        {
            ViewBag.countries = new SelectList(await _service.Get<Country>(), "ID", "Name");
            ViewBag.countrycodes = new AddressDTO().getCodes();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Disable2FA(string url,string Id)
        {
            if (Id != null)
            {
                var res = await userManager.SetTwoFactorEnabledAsync(await userManager.FindByIdAsync(Id),false);
            }
            return Redirect(url);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> notifyLogin(string Id)
        {
            var user = await userManager.FindByIdAsync(Id.Split('/').First());
            if (user != null)
            {
                if (Id.Split('/').Last() == "1")
                {
                    user.IsActive = true;
                }
                else
                {
                    user.IsActive = false;

                }
                var checkl = await userManager.UpdateAsync(user);
            }
            return RedirectToAction("SecurityInfo");
        }
        [HttpPost]
        public async Task<IActionResult> AddAddress(AddressDTO model)
        {
            var user = await _Repo.GetAsync<ProfileDTO, ApplicationUser>(x => x.Id == Id,true);
            user.Addresses!.Add(model);
            var check = await _Repo.UpdateAsync<ProfileDTO, ApplicationUser>(user);
            return RedirectToAction("Address");
        }
        public async Task<IActionResult> DeleteAddress()
        {
            var profile = JsonConvert.DeserializeObject<ProfileDTO>((string)TempData["ProfileJson"]!);
            await _service.Delete(profile!.Addresses![0].Id);
            return RedirectToAction("Address");
        }
        public async Task<IActionResult> DefaultAddress()
        {
            var profile = JsonConvert.DeserializeObject<ProfileDTO>((string)TempData["ProfileJson"]!);
            var check = await _service.Default(profile!.Addresses![0].Id);
            return RedirectToAction("Address");
        }
        public async Task<IActionResult> UpdateAddress()
        {
            var profile = JsonConvert.DeserializeObject<ProfileDTO>((string)TempData["ProfileJson"]!);

            var model = await _Repo.GetAsync<ProfileDTO, ApplicationUser>(x => x.Id == profile!.Id, true, 
                inc => inc.Include(add => add.Addresses!.Where(a => a.Id == profile.Addresses[0].Id))
                .ThenInclude(are => are.Area)
                .ThenInclude(cit => cit!.City)
                .ThenInclude(cou => cou!.Country));
            ViewBag.countries = new SelectList(await _service.Get<Country>(), "ID", "Name");
            ViewBag.countrycodes = new AddressDTO().getCodes();
            return View(model.Addresses![0]);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAddress(AddressDTO model)
        {
            await Console.Out.WriteLineAsync((await _Repo.UpdateAsync<AddressDTO,Address>(model)).ToString());
            return RedirectToAction("Address");
        }
        public async Task<IActionResult> DownloadProfile(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var SheetName = user.FirstName == null ? "YourSheet": user.FirstName;
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add(SheetName);
                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Email";
                worksheet.Cell(1, 3).Value = "Phone Number";
                worksheet.Cell(1, 4).Value = "IsActive";
                worksheet.Cell(1, 5).Value = "Email Confiremed";
                worksheet.Cell(1, 6).Value = "Photo";

                worksheet.Cell(2, 1).Value = user.FirstName + " " + user.LastName;
                worksheet.Cell(2, 2).Value = user.Email;
                worksheet.Cell(2, 3).Value = user.PhoneNumber;
                worksheet.Cell(2, 4).Value = user.IsActive;
                worksheet.Cell(2, 5).Value = user.EmailConfirmed;
                worksheet.Cell(2, 6).Value = user.ProfilePhoto;

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "Export.xlsx");
            }
            return View();
        }
        
        public async Task<IActionResult> SecurityInfo()
        {

            ViewBag.ActiveTab = "SecurityInfo";
            var model = await _Repo.GetAsync<ProfileDTO,ApplicationUser>(x => x.Id == Id);
            await Console.Out.WriteLineAsync(model.LoginHistory.Count.ToString());

            return View(model);

        }
        public async Task<IActionResult> Notification()
        {

            ViewBag.ActiveTab = "Notification";
            var model = await _Repo.GetAsync<ProfileDTO, ApplicationUser>(x => x.Id == Id);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {

            ViewBag.ActiveTab = "OrderHistory";
            var model = await _Repo.GetAsync<ProfileDTO, ApplicationUser>(x => x.Id == Id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileDTO model , string url)
        {

            if (ModelState.TryGetValue(nameof(model.PhotoFile), out var entry) && entry.Errors.Count == 0)
            {
                // Handle photo upload
                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(model.PhotoFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("PhotoFile", "Invalid file type. Only JPG, PNG, and GIF files are allowed.");
                        return View(model);
                    }
                    // Validate file size (e.g., max 5MB)
                    if (model.PhotoFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("PhotoFile", "File size cannot exceed 5MB.");
                        return View(model);
                    }
                    // Generate unique filename
                    var fileName = Guid.NewGuid().ToString() + Path.GetFileName(model.PhotoFile.FileName);
                    var folder = Directory.GetCurrentDirectory() + @"\wwwroot\uploads\photos\";
                   

                    // Ensure directory exists
                    //Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(folder, fileName);

                    //Delete old photo if exists
                    if (model.SavedPhotos == null)
                    {
                        model.SavedPhotos = new List<string>();
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.PhotoFile.CopyToAsync(fileStream);
                        }
                        model.SavedPhotos.Add(model.PhotoFile.FileName+'|'+fileName);
                        model.PhotoPath = "/uploads/photos/" + fileName;
                    }
                    else
                    {
                        bool chk = false;
                       for(int f = 0;f<model.SavedPhotos.Count;f++) 
                        {
                            if (model.SavedPhotos[f].Split('|')[0] == model.PhotoFile.FileName)
                            {
                                model.PhotoPath = "/uploads/photos/" + model.SavedPhotos[f].Split('|')[1];
                                chk = true; break;
                            }
                        }
                        if (!chk)
                        {
                            // Save file
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await model.PhotoFile.CopyToAsync(fileStream);
                            }
                            model.SavedPhotos.Add(model.PhotoFile.FileName + '|' + fileName);
                            model.PhotoPath = "/uploads/photos/" + fileName;
                        }
                    }
                    // Update model with new photo path


                    var updated = await _Repo.GetAsync<ProfileDTO, ApplicationUser>(x => x.Id == model.Id);
                    updated.PhotoPath = model.PhotoPath;
                    updated.SavedPhotos = model.SavedPhotos;
                    model = updated;
                }

                // Save to database
                var check = await _Repo.UpdateAsync<ProfileDTO, ApplicationUser>(model);
                TempData["Success"] = "Profile updated successfully!";
            }
            else
            {
                var errorMessages = entry.Errors.Select(e => e.ErrorMessage).ToList();
                Console.WriteLine(errorMessages);
            }
            return RedirectToAction(url);
        }
        [HttpPost]
        public async Task<IActionResult> SendOtpAjax([FromBody] ProfileDTO request)
        {
            Console.WriteLine(request.PhoneNumber+"dsad");
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid phone number" });
            }
            var result = await _otpService.SendOtpAsync(request.PhoneNumber);
            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message,
                sessionId = result.SessionId
            });
        }
        [HttpPost]
        public JsonResult VerifyOtpAjax([FromBody] VerifyOtpAjaxRequest request)
        {
            var result =  _otpService.VerifyOtp(request.SessionId, request.PhoneNumber, request.OtpCode);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }
        [HttpPost]
        public async Task<IActionResult> GetCities(int Id)
        {
            var allCities = await _Repo.GetAsync<City>(x => x.CountryId == Id);

            return Json(allCities);
        }
        [HttpPost]
        public async Task<JsonResult> GetAreas(int Id)
        {
            var allAreas = await _Repo.GetAsync<Area>(x => x.CityId == Id);

            return Json(allAreas);
        }
        public IActionResult ChangePass()
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var modelPassword = new ChangePasswordDTO
                {
                    Id = userId,
                };
                return View(modelPassword);

            }
            return View(new ChangePasswordDTO());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAcc()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePass(ChangePasswordDTO model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    var check = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
                    if (check.Succeeded)
                    {
                        ViewBag.Ok = 1;
                        await _Repo.UpdateAsync<ChangePasswordDTO, ApplicationUser>(model);
                        //TempData["changeTime"] = JsonConvert.SerializeObject(model.ChangeTime);
                        return View();
                    }
                    else
                    {
                        ViewBag.Ok = 0;
                        foreach (var item in check.Errors)
                        {
                            ModelState.AddModelError(item.Code,item.Description);
                        }
                    }
                }
            }
            return View(model);
        }
    }
    public class VerifyOtpAjaxRequest
    {
        public string SessionId { get; set; }
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
    }
}
