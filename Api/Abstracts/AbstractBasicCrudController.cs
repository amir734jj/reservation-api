using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Abstracts
{
    [Route("api/[controller]")]
    public abstract class AbstractBasicCrudController<T> : Controller where T: IEntity
    {
        /// <summary>
        ///     Abstract logic layer
        /// </summary>
        /// <returns></returns>
        protected abstract IBasicCrudLogic<T> ResolveLogic();

        /// <summary>
        ///     Returns all questions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<IEntity>), 200)]
        [SwaggerOperation("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await ResolveLogic().GetAll());
        }

        /// <summary>
        ///     Get question by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(IEntity), 200)]
        [SwaggerOperation("Get")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            return Ok(await ResolveLogic().Get(id));
        }

        /// <summary>
        ///     Update a question given Id and DTO
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(IEntity), 200)]
        [SwaggerOperation("Update")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] T instance)
        {
            return Ok(await ResolveLogic().Update(id, instance));
        }

        /// <summary>
        ///     Delete a question given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(IEntity), 200)]
        [SwaggerOperation("Delete")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            return Ok(await ResolveLogic().Delete(id));
        }

        /// <summary>
        ///     Save a question given DTO
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(IEntity), 200)]
        [SwaggerOperation("Save")]
        public async Task<IActionResult> Save([FromBody] T instance)
        {
            return Ok(await ResolveLogic().Save(instance));
        }
    }
}