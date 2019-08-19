using Api.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Api.Controllers
{
    public class ItemController : AbstractBasicCrudController<Item>
    {
        private readonly IItemLogic _itemLogic;

        public ItemController(IItemLogic itemLogic)
        {
            _itemLogic = itemLogic;
        }
        
        protected override IBasicCrudLogic<Item> ResolveLogic()
        {
            return _itemLogic;
        }
    }
}