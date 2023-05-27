using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
        public DishDto GetById(int restaurantDto, int dishDto);
        public List<DishDto> GetAll(int restaurantId);
        public void RemoveAll(int restaurantId);
        public void RemoveOne(int restaurantId, int dishId);
    }

    public class DishService : IDishService
    {
        private readonly IMapper _mapper;
        private readonly RestaurantDbContext _context;

        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishEntity = _mapper.Map<Dish>(dto);

            dishEntity.RestaurantId= restaurantId;

            _context.Dishes.Add(dishEntity);

            _context.SaveChanges();

            return dishEntity.Id;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            if (restaurant is null )
                throw new NotFoundException("Dish not found");

            var dish = _context.Dishes.FirstOrDefault(d => d.Id == dishId);

            if(dish is null|| dish.RestaurantId!=restaurantId)
                throw new NotFoundException("Dish not found");

            var dishDto=_mapper.Map<DishDto>(dish);

            return dishDto;
        }
        
        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishDto=_mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishDto;
            
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            _context.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();
        }

        public void RemoveOne(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            if (restaurant is null )
                throw new NotFoundException("Dish not found");

            var dish=restaurant.Dishes.FirstOrDefault(d=>d.Id==dishId);

            if (dish is null || dish.RestaurantId != dishId)
                throw new NotFoundException("Dish not found");
            _context.Remove(dish);
            _context.SaveChanges(); 
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = (_context
                .Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restaurantId));

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");
            return restaurant;
        }
    }
}
