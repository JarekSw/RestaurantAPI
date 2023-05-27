
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections.Generic;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController: ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }
        [HttpPost]
        public ActionResult Post([FromRoute]int restaurantId, [FromBody]CreateDishDto dto)
        {
            var newDishId=_dishService.Create(restaurantId,dto);

            return Created($"api/restaurant/{restaurantId}/dish/{newDishId}",null);
        }

        [HttpGet]
        public ActionResult<List<DishDto>> GetAll([FromRoute] int restaurantId)
        {
            var dishDto = _dishService.GetAll(restaurantId);

            return Ok(dishDto);
        }
        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute] int restaurantId, [FromRoute]int dishId)
        {
            var dishDto = _dishService.GetById(restaurantId, dishId);

            return Ok(dishDto);
        }

        [HttpDelete]
        public ActionResult DeleteAll([FromRoute]int restaurantId)
        {
            _dishService.RemoveAll(restaurantId);

            return NoContent();
        }


        [HttpDelete("{dishId}")]
        public ActionResult Delete([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            _dishService.RemoveOne(restaurantId,dishId);

            return NoContent();
        }

    }
}
