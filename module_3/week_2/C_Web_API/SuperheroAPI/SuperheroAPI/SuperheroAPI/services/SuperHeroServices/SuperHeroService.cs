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

