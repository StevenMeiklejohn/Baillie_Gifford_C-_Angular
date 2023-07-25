# CRUD Using Web API Entity Core

## Overview
In this lesson we'll build a CRUD app implementing a web API using .NET Entity Core.


## Setting Up.
We will require a database for this project so start up the SQL server using Docker as we have done in the previous lessons.
Next, lets start a new Visual Studio project.
Select Web Application (Model-View-Controller)
Select .NET core 7.0, No Authentication and ensure that 'configure for HTTPS' is selected.


## Creating a model.
Lets create a class to store in our db and serve up with the controller.
Right click on SuperHeroAPI and create a folder called models.
Inside this folder create a new class called Superhero.

```
using System;
namespace SuperheroAPI.Models
{
	public class Superhero
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Place { get; set; }


        public Superhero()
		{
		}
	}
}
```

## Create the controller.
Right click on the controller folder and select add/ASP.NETCore/Controller Class.
Name the file SuperheroController.
Change the Controller from which we inherit to ControllerBase. Controller provides functionality for making views (MVC) which we won't be using here so we can use the Base Controller.
```
namespace SuperheroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperheroController : ControllerBase
    {

    }
}
```
The first line of the controller describes the route we will use to hit our controller routes.
In this case the routes will be "api/Superhero". The [controller] in the brackets portion of this line denotes the part of the controller file name which will EXCLUDED from route, which seems kind of counter intuitive.

## Implementing Routes.
Lets create our first get route;
```
        [HttpGet]
        public async Task<ActionResult<List<Superhero>>> GetAllHeroes()
        {
            var superheroes = new List<Models.Superhero>
            {
                new Models.Superhero
                    {
                    Id = 1,
                    Name="Spider Man",
                    FirstName="Peter",
                    LastName="Parker",
                    Place="New York"
                    },
                new Superhero
                    {
                    Id = 2,
                    Name="Iron Man",
                    FirstName="Tony",
                    LastName="Stark",
                    Place="Malibu"
                    }
            };
            return Ok(superheroes);
        }
```
Now if we run the app and go to "https://localhost:7048/api/Superhero" in the browser we should see our spiderman object presented as JSON.
We have our first route!
As we are creating our other routes it might get a bit annoying having to create new Superhero object all the time, so lets abstract that out. (Obviously we just hard coding dummy data at this point, we will implement the db later.)
```
namespace SuperheroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperheroController : ControllerBase
    {
        private static List<Superhero> superHeroes = new List<Superhero> {
             new Superhero
                 {
                    Id = 1,
                    Name="Spider Man",
                    FirstName="Peter",
                    LastName="Parker",
                    Place="New York"
                 },
                new Superhero
                    {
                    Id = 2,
                    Name="Iron Man",
                    FirstName="Tony",
                    LastName="Stark",
                    Place="Malibu"
                    }
            };

        [HttpGet]
        public async Task<ActionResult<List<Superhero>>> GetAllHeroes()
        {
            return Ok(superHeroes);
        }
    }
}
```
Now lets create the route to get a specific superhero.
```
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Superhero> GetSingleHero(int id)
        {
            var hero = superHeroes.Find(x => x.Id == id);
            return Ok(hero);
        }
```
In this case we need to extract the id from the url. We can do this by adding to our route definition. So the new route will be "https://localhost:7048/api/Superhero/1" where the 1 will be known as "id".
Since we are now searching for a specific hero, we should probably implement the ability to inform the user if the given id does not exist.
```
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Superhero> GetSingleHero(int id)
        {
            var hero = superHeroes.Find(x => x.Id == id);
            if(hero is null)
            {
                return NotFound("Sorry, but this hero doesn't exist");
            }
            return Ok(hero);
        }
```

Next, lets create a post request to add a hero;
```
        [HttpPost]
        public async Task<ActionResult<List<Superhero>>> AddHero(Superhero hero)
        {
            superHeroes.Add(hero);
            return Ok(superHeroes);
        }
```
We can test this route using Insomnia.

Lets add the route to update a hero;
```
        [HttpPut]
        public async Task<ActionResult<List<Superhero>>> UpdateHero(Superhero request)
        {
            var hero = superHeroes.Find(x => x.Id == request.Id);
            if (hero is null)
            {
                return NotFound("Sorry, but this hero doesn't exist");
            }
            hero.Name = request.Name;
            hero.FirstName = request.FirstName;
            hero.LastName = request.LastName;
            hero.Place = request.Place;
            return Ok(superHeroes);
        }
```
It should be noted that with our current put route we need to provide all the information related to a superhero even if we only want to change one value. If a piece of superhero information is not supplied the vale will be set to 'string'.

Finally, lets add the delete route;
```
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<List<Superhero>>> DeleteHero(int id)
        {
            var hero = superHeroes.Find(x => x.Id == id);
            if (hero is null)
            {
                return NotFound("Sorry, but this hero doesn't exist");
            }
            superHeroes.Remove(hero);
            return Ok(superHeroes);
        }
```



## Repository Pattern
At the moment all the logic associated with our routes resides in the controller. This is not ideal, so it might be worth abstracting away that logic into a "services" class. Another benefit of this is that we will be able to swap out strategies (i.e. switching from hard coded data to a db) without modifying the controller.
We will create some 'services' and then 'inject them into our controller using dependency injection.
At the same level as our models folder, lets create a folder called "services" and inside that create a folder called "SuperheroService".
Inside this folder create an interface called ISuperHeroService and a new class called SuperHeroService.
Our SuperHeroService class will then implement the ISuperHeroService interface.
```
using System;
namespace SuperheroAPI.services.SuperHeroServices
{
	public class SuperHeroService : ISuperHeroService
	{
		public SuperHeroService()
		{
		}
	}
}
```
So, what should we put into our new interface?
We're going to add the function definition of each function that we require in the controller. We will then implement those functions in the SuperheroService class (which implements the interface).
Note; Add the following line to the program.cs file. This will make importing classes much quicker.
```
global using SuperheroAPI.Models;
```

Our updated interface should look like this;
```
using System;
namespace SuperheroAPI.services.SuperHeroServices
{
	public interface ISuperHeroService
	{
		List<Superhero> GetAllHeroes();

		Superhero? GetSingleHero(int id);

		List<Superhero> AddSuperHero(Superhero hero);

		List<Superhero>? UpdateHero(Superhero hero);

		List<Superhero>? DeleteHero(int id);
	}
}
```
Now we can implement these functions in our HeroServices class;
(Notice we are no longer return the OkResult object. Also we are unable to return the NotFound object. For the moment we will return null and check for this in the controller).
```
using System;

namespace SuperheroAPI.services.SuperHeroServices
{
	public class SuperHeroService : ISuperHeroService
	{

        private static List<Superhero> superHeroes = new List<Superhero> {
             new Superhero
                 {
                    Id = 1,
                    Name="Spider Man",
                    FirstName="Peter",
                    LastName="Parker",
                    Place="New York"
                 },
             new Superhero
                 {
                    Id = 2,
                    Name="Iron Man",
                    FirstName="Tony",
                    LastName="Stark",
                    Place="Malibu"
                 }
        };
        public SuperHeroService()
		{
		}

        public List<Superhero> AddSuperHero(Superhero hero)
        {
            superHeroes.Add(hero);
            return superHeroes;
        }

        public List<Superhero> DeleteHero(int id)
        {
            var hero = superHeroes.Find(x => x.Id == id);
            if (hero is null)
            {
                return null;
            }
            superHeroes.Remove(hero);
            return superHeroes;
        }

        public List<Superhero> GetAllHeroes()
        {
            return superHeroes;
        }

        public Superhero GetSingleHero(int id)
        {
            var hero = superHeroes.Find(x => x.Id == id);
            if (hero is null)
            {
                return null;
            }
            return hero;
        }

        public List<Superhero> UpdateHero(Superhero request)
        {
            var hero = superHeroes.Find(x => x.Id == request.Id);
            if (hero is null)
            {
                return null;
            }
            hero.Name = request.Name;
            hero.FirstName = request.FirstName;
            hero.LastName = request.LastName;
            hero.Place = request.Place;
            return superHeroes;
        }
    }
}
```
Now we can update our controller.
Notice at the top of the file where we add a constructor for our controller with assigns an instance of our SuperHeroService class, which implements our ISuperheroService interface.

```
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
```
If we run the service at this point you should see an error something like this.
```
An unhandled exception occurred while processing the request.
InvalidOperationException: Unable to resolve service for type 'SuperheroAPI.services.SuperHeroServices.ISuperHeroService' while attempting to activate 'SuperheroAPI.Controllers.SuperheroController'.
Microsoft.Extensions.DependencyInjection.ActivatorUtilities.GetService(IServiceProvider sp, Type type, Type requiredBy, bool isDefaultParameterRequired)
```
What we need to do to fix this is 'register' our new class and interface for use within the wider program. Add the following line to the Program.cs

```
builder.Services.AddScoped<ISuperHeroService, SuperHeroService>();
```

What we are doing here is telling the program that anywhere we try to inject the ISuperHeroService interface, we should use the implementation provided by the SuperHeroService class. The beauty of this is that we can easily change which class the interface should obtain its implementation from. This will be handy when we switch to using a database.

Restarting the app again should resolve the error.

### Ask the class to re-check all routes using insomnia.


# Adding the db component (alt method)
========================================
========================================













