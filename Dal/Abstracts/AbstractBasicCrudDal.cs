using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgileObjects.AgileMapper;
using Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Interfaces;

namespace Dal.Abstracts
{
    public abstract class BasicCrudDalAbstract<T> : IBasicCrudDal<T> where T : class, IEntity
    {        
        /// <summary>
        /// Abstract to get database context
        /// </summary>
        /// <returns></returns>
        protected abstract DbContext GetDbContext();
        
        /// <summary>
        /// Abstract to get entity set
        /// </summary>
        /// <returns></returns>
        protected abstract DbSet<T> GetDbSet();

        /// <summary>
        /// DTO to Entity mapper
        /// </summary>
        /// <returns></returns>
        protected abstract IMapper Mapper();

        /// <summary>
        /// Returns all entities
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await GetDbSet().ToListAsync();
        }

        /// <summary>
        /// Returns an entity given the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Get(Guid id)
        {
            return await GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Saves an instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<T> Save(T instance)
        {
            await GetDbSet().AddAsync(instance);

            await GetDbContext().SaveChangesAsync();

            return instance;
        }

        /// <summary>
        /// Deletes entity given the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Delete(Guid id)
        {
            var entity = await GetDbSet().FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {
                // Remove from persistence
                GetDbSet().Remove(entity);
                
                // Remove form DbContext
                GetDbContext().Remove(entity);
                
                await GetDbContext().SaveChangesAsync();
                
                return entity;
            }

            return null;
        }

        /// <summary>
        /// Handles update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(Guid id, T dto)
        {
            if (dto != null)
            {
                var entity = await Get(id);

                Mapper().Map(dto).OnTo(entity);

                // Update
                GetDbSet().Update(entity);
                
                // Save and dispose
                await GetDbContext().SaveChangesAsync();

                // Returns the updated entity
                return dto;
            }

            // Not found
            return null;
        }

        /// <summary>
        /// Handles manual update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifyAction"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(Guid id, Action<T> modifyAction)
        {            
            var entity = await GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
                
            if (entity != null)
            {
                // Update
                modifyAction(entity);

                GetDbSet().Update(entity);
                
                // Save and dispose
                await GetDbContext().SaveChangesAsync();

                // Returns the updated entity
                return entity;
            }

            // Not found
            return null;
        }
    }
}