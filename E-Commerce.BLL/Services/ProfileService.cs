using E_Commerce.BLL.Models.ProfileModels;
using E_Commerce.DAL.Entity_Extension;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services
{
    public class ProfileService : IProfileService
    {
        //private readonly UserManager<ApplicationUser> _manager;
        //private readonly ILogger<ProfileService> logger;
        //public ProfileService(UserManager<ApplicationUser> _manager , ILogger<ProfileService> logger)
        //{
        //    this._manager = _manager;
        //    this.logger = logger;
        //}
        ////public async Task<ProfileHead> LoadHeading(string Id)
        ////{
           
        ////    try
        ////    {
        ////        var user = await _manager.FindByIdAsync(Id);
        ////        if (user == null)
        ////        {
        ////            logger.LogWarning("user not exist during LoadHeading: {Email}", user.Id); 
        ////        }
        ////        else
        ////        {
        ////            var _ProfileHead = new ProfileHead()
        ////            {
        ////                Id = user.Id,
        ////                E_mail = user.Email,
        ////                Name= user.FirstName+user.LastName,
        ////                PhotoPath=user.ProfilePhoto,
        ////            };
        ////            return _ProfileHead;
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        logger.LogError(e, "Error during LoadHeading");
        ////    }
        ////    return null;
        ////}
    }
    public interface IProfileService
    {
        //public Task<ProfileHead> LoadHeading(string Id);
    }
}
