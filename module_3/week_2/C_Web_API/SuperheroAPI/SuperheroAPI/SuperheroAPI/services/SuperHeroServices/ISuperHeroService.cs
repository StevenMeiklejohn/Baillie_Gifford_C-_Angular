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

