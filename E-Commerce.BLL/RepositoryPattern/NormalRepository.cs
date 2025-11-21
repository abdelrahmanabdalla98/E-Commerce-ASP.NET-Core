using AutoMapper;
using E_Commerce.BLL.Repository;
using E_Commerce.DAL.DB_Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.RepositoryPattern
{
    public class NormalRepository<DTO, T> : INormalRepository<DTO, T> where T : class, new() where DTO : class, new()
    {

        private readonly ApplicationContext Context;
        private readonly IMapper _Mapper;
        public NormalRepository(ApplicationContext Context, IMapper Mapper)
        {
            this.Context = Context;
            _Mapper = Mapper;
        }
        public async Task<bool> DeleteAsync(DTO model)
        {

            try
            {
                var query = Context.Set<T>().Remove(_Mapper.Map<T>(model));
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
                return false;
            }
        }

        public async Task<IEnumerable<DTO>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null
            , int? PageSize = null
            , int? PageNum = null
            , bool notrack = false
            , params Expression<Func<T, object>>[] includes)
        {
            var query = Context.Set<T>().AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (PageSize.HasValue && PageNum.HasValue)
            {
                query = query.Skip((int)((PageNum - 1) * PageSize)).Take((int)PageSize).Distinct().OrderBy(filter);
            }
            if (notrack == true)
            {
                query = query.AsNoTracking();
            }
            foreach (var item in includes)
            {
                query = query.Include(item);
            }
            return _Mapper.Map<IEnumerable<DTO>>(await query.ToListAsync());

        }
        public async Task<DTO> GetAsync(Expression<Func<T, bool>> filter
            , bool notrack = false
            , Func<IQueryable<T>, IQueryable<T>>? includeQuery = null)
        {
            var query = Context.Set<T>().Select(x => x);
            if (filter != null)
            {
                query = query.Where(filter);

            }
            //foreach (var item in includes)
            //{
            //    query = query.Include(item);
            //}
            if (includeQuery != null)
            {
                query = includeQuery(query);
            }
            if (notrack == true)
            {
                query = query.AsNoTracking();
            }

            return _Mapper.Map<DTO>(await query.FirstOrDefaultAsync());
        }

        public async Task<bool> UpdateAsync(DTO model)
        {
            try
            {
                // Get primary key name
                var key = Context.Model.FindEntityType(typeof(T))
                                ?.FindPrimaryKey()
                                ?.Properties
                                ?.FirstOrDefault()
                                ?.Name;

                if (key == null)
                    throw new InvalidOperationException("Primary key not found for entity.");

                // Extract the key value from the DTO
                var keyValue = model?.GetType().GetProperty(key)?.GetValue(model);
                if (keyValue == null)
                    throw new InvalidOperationException("DTO does not contain a valid key value.");

                // Find the existing entity in the database
                var existingEntity = await Context.Set<T>().FindAsync(keyValue);
                if (existingEntity == null)
                    return false; // No entity found to update

                // Map updated values onto the existing entity
                _Mapper.Map(model, existingEntity);

                // Save changes
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Optional: log exception
                Console.WriteLine($"UpdateAsync failed: {ex.Message}");
                return false;
            }
        }

        //public async Task<bool> Update(DTO model, dynamic Id)
        //{
        //    try
        //    {
        //        // Fetch the tracked entity from the database
        //        var existingEntity = await Context.Set<T>().FindAsync(Id);
        //        if (existingEntity == null)
        //            return false;
        //        // Overlay the DTO values onto the tracked entity
        //        _Mapper.Map(model, existingEntity);
        //        // EF will detect changes and persist them
        //        await Context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Update failed: {ex.Message}");
        //        return false;
        //    }
        //}
        public async Task<bool> CreateAsync(DTO model)
        {
            try
            {
                var query = await Context.Set<T>().AddAsync(_Mapper.Map<T>(model));
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message.ToString());
                return false;
            }

        }

    }
    public interface INormalRepository<DTO, T> where T : class, new() where DTO : class, new()
    {
        public Task<IEnumerable<DTO>> GetAllAsync(
           Expression<Func<T, bool>>? filter = null
           , int? PageSize = null
           , int? PageNum = null
           , bool notrack = false
           , params Expression<Func<T, object>>[] includes);

        public Task<DTO> GetAsync(Expression<Func<T, bool>> filter
            , bool notrack = false
            , Func<IQueryable<T>, IQueryable<T>>? includeQuery = null);

        public Task<bool> UpdateAsync(DTO model);
        public Task<bool> CreateAsync(DTO model);
        public Task<bool> DeleteAsync(DTO model);


    }
}
