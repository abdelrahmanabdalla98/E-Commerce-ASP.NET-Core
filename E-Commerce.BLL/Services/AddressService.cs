using AutoMapper;
using E_Commerce.BLL.Models.ApplicationModels;
using E_Commerce.DAL.DB_Context;
using E_Commerce.DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_Commerce.BLL.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationContext context;
        private readonly IMapper mapper;

        public AddressService(ApplicationContext _context ,IMapper mapper)
        {
            context = _context;
            this.mapper = mapper;
        }


        //public async Task<IEnumerable<Area>> areas(Expression<Func<Area, bool>> filter)
        //{
        //    if (filter != null)
        //    {
        //        return await context.Set<Area>().Where(filter).ToListAsync();
        //    }
        //    return await context.Set<Area>().ToListAsync();
        //}

        //public async Task<IEnumerable<City>> cities(Expression<Func<City, bool>> filter)
        //{
        //    if (filter != null)
        //    {
        //        return await context.Set<City>().Where(filter).ToListAsync();
        //    }
        //    return await context.Set<City>().ToListAsync();
        //}

        //public async Task<IEnumerable<Country>> countries(Expression<Func<Country, bool>>? filter = null)
        //{
        //    if (filter != null)
        //    {
        //        return await context.Set<Country>().Where(filter).ToListAsync();
        //    }
        //    return await context.Set<Country>().ToListAsync();
        //}
        //public async Task<Area> GetArea(Expression<Func<Area, bool>> filter)
        //{
        //    var area = await context.Set<Area>().Where(filter).Include(ct => ct.City)
        //        .ThenInclude(co => co!.Country).FirstOrDefaultAsync();
        //    return area!;
        //}
        public async Task<bool> Default(int Id)
        {
            try
            {
                var _default = await context.Set<Address>().Where(x => x.IsDefault == true).FirstOrDefaultAsync();
                if(_default != null)
                {
                    _default.IsDefault = false;
                }
                var model = await context.Set<Address>().Where(x => x.Id == Id).FirstOrDefaultAsync();
                model.IsDefault = true;
                await context.SaveChangesAsync();
                return true;
            }
            catch(Exception) {
                return false;
            }
        }

        public async Task<bool> Delete(int Id)
        {
            try
            {
                var model=await context.Set<Address>().Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (model != null)
                {
                    if (model.IsDefault)
                    {
                        var updateD = await context.Set<Address>().Where(x => x.IsDefault != true && x.IsDeleted!=true).FirstOrDefaultAsync();
                        if (updateD != null)
                        {
                            updateD.IsDefault = true;
                        }
                    }
                    model!.IsDefault = false;
                    model!.IsDeleted = true;
                    await context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<T>> Get<T>(Expression<Func<T, bool>>? filter = null) where T : class
        {
            if (filter != null)
            {
                return await context.Set<T>().Where(filter).ToListAsync();
            }
            return await context.Set<T>().ToListAsync();
        }

        public async Task<bool> Update(AddressDTO model)
        {
            try
            {
                if (model.IsDefault)
                {
                    var Defobj = await context.Set<Address>().Where(def =>def.IsDefault==true).FirstOrDefaultAsync();
                    if (Defobj != null)
                    {
                        Defobj.IsDefault = false;
                    }
                }
                var obj = context.Set<Address>().Update(mapper.Map<Address>(model));
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public interface IAddressService
    {
        public Task<IEnumerable<T>> Get<T>(Expression<Func<T, bool>>? filter = null) where T : class;
        //public Task<IEnumerable<Country>> countries(Expression<Func<Country, bool>>? filter = null);
        //public Task<IEnumerable<City>> cities(Expression<Func<City, bool>> filter);
        //public Task<IEnumerable<Area>> areas(Expression<Func<Area, bool>> filter);
        //public Task<Area> GetArea(Expression<Func<Area, bool>> filter);
        public Task<bool> Update(AddressDTO model);
        public Task<bool> Delete(int Id);
        public Task<bool> Default(int Id);
    }

}
