using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperheroAPI.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperheroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperheroController : ControllerBase
    {
        private readonly ISuperHeroService _superHeroService;

        public SuperheroController(ISuperHeroService superHeroService)
        {
            _superHeroService = superHeroService;
        }
        

        [HttpGet]
        public async Task<ActionResult<Superhero>> GetAllHeroes()
        {
            var result = _superHeroService.GetAllHeroes();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<List<Superhero>>> GetSingleHero(int id)
        {
            var result = _superHeroService.GetSingleHero(id);
            if(result is null)
            {
                return NotFound("Sorry, but this hero doesn't exist");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<Superhero>>> AddHero(Superhero hero)
        {
            var result = _superHeroService.AddSuperHero(hero);
            return Ok(result);
        }


        [HttpPut]
        public async Task<ActionResult<List<Superhero>>> UpdateHero(Superhero request)
        {
            var result = _superHeroService.UpdateHero(request);
            if (result is null)
            {
                return NotFound("Hero not found");
            }
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<List<Superhero>>> DeleteHero(int id)
        {
            var result = _superHeroService.DeleteHero(id);
            if(result is null)
            {
                return NotFound("Hero not found");
            }
            return Ok(result);
        }
    }
}

