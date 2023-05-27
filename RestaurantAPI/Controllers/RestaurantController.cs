using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    public class RestaurantController:ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;          
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody]UpdateRestaurantDto dto,[FromRoute]int id)
        {
            _restaurantService.Update(id, dto);
            
            return Ok();          
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute]int id)
        {
            _restaurantService.Delete(id);
            
            return NoContent();      
        }

        [HttpPost]
        public ActionResult CreateRestaurant([FromBody]CreateRestaurantDto dto)
        {
            var id= _restaurantService.Create(dto);

            return Created($"api/restaurant/{id}",null);
        }


        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> GettAll()
        {
            var restaurantsDtos= _restaurantService.GettAll();

            return Ok(restaurantsDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> GetAction([FromRoute]int id)
        {
            var restaurant=_restaurantService.GetById(id);
                     
            return Ok(restaurant);
        }
    }
}
