using Dal.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Cruds
{
    public class ItemLogic : AbstractBasicCrudLogic<Item>, IItemLogic
    {
        private readonly IItemDal _itemDal;

        public ItemLogic(IItemDal itemDal)
        {
            _itemDal = itemDal;
        }
        
        protected override IBasicCrudDal<Item> ResolveDal()
        {
            return _itemDal;
        }
    }
}