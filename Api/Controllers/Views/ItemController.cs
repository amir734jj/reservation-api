using System;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace Api.Controllers.Views
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class ItemController : Controller
    {
        private readonly IItemLogic _itemLogic;

        public ItemController(IItemLogic itemLogic)
        {
            _itemLogic = itemLogic;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var items = await _itemLogic.GetAll();
            
            return View(items);
        }

        [HttpGet]
        [Route("Save")]
        public async Task<IActionResult> Save()
        {
            return View();
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> SaveHandler(Item item)
        {
            await _itemLogic.Save(item);
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _itemLogic.Get(id);
            
            return View(item);
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditHandler(Item item)
        {
            await _itemLogic.Update(item.Id, item);

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _itemLogic.Delete(id);
            
            return RedirectToAction("Index");
        }
    }
}