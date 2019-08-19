using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dal.Interfaces;
using Logic.Interfaces;
using Models.Interfaces;

namespace Logic.Abstracts
{
    public abstract class AbstractBasicCrudLogic<T> : IBasicCrudLogic<T> where T: IEntity
    {
        protected abstract IBasicCrudDal<T> ResolveDal();
        
        public Task<IEnumerable<T>> GetAll()
        {
            return ResolveDal().GetAll();
        }

        public Task<T> Get(Guid id)
        {
            return ResolveDal().Get(id);
        }

        public Task<T> Save(T instance)
        {
            return ResolveDal().Save(instance);
        }

        public Task<T> Delete(Guid id)
        {
            return ResolveDal().Delete(id);
        }

        public Task<T> Update(Guid id, T updatedInstance)
        {
            return ResolveDal().Update(id, updatedInstance);
        }

        public Task<T> Update(Guid id, Action<T> modifyAction)
        {
            return ResolveDal().Update(id, modifyAction);
        }
    }
}