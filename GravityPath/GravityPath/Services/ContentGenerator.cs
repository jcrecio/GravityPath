namespace GravityPath.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EntityGame;

    public class ContentGenerator
    {
        private static ContentGenerator _contentGenerator;

        public static ContentGenerator GetInstance()
        {
            return _contentGenerator ?? (_contentGenerator = new ContentGenerator());
        }

        private ContentGenerator()
        {
        }

        public IEnumerable<PlanetFiller> GeneratePlanets(int positionY, int amountToGenerate)
        {
            var listPlanets = new List<PlanetFiller>();

            int indexY = positionY;

            for (int i = 0; i < amountToGenerate; i++)
            {
                var planet = this.GeneratePlanet(indexY);
                listPlanets.Add(planet);

                indexY += (new Random()).Next(800, 1500);
            }

            return listPlanets;
        }

        public PlanetFiller GeneratePlanet(int indexY)
        {
            var mass = (new Random()).Next(75, 450);
            var radius = (new Random()).Next(mass/2, mass);

            var leftOrRight = (new Random()).Next(0, 2);
            int halfRadius = radius/2;
            int x = leftOrRight == 0 
                ? new Random().Next(-50, 400 - radius/7) 
                : new Random().Next(120 + radius/10, 480 + halfRadius/2);

            int y = indexY - halfRadius;

            var planet = new PlanetFiller(radius, radius, mass, x, y);
            return planet;
        }

        public IEnumerable<DangerFiller> GenerateDangerSignals(IEnumerable<PlanetFiller> listNewPlanetsToCreate)
        {
            var dangerFillers = listNewPlanetsToCreate.Select(p => this.GenerateDangerSignal(p));
            return dangerFillers;
        }

        public DangerFiller GenerateDangerSignal(PlanetFiller planetFiller)
        {
            return new DangerFiller{
                X0 = planetFiller.PositionX - planetFiller.DimensionX / 2f,
                CenterX = planetFiller.PositionX,
                X1 = planetFiller.PositionX + planetFiller.DimensionX / 2f,
                Diameter = planetFiller.DimensionX,
                RangeBottom = planetFiller.PositionY - 400 ,
                RangeTop = planetFiller.PositionY - 3000//here
            };
        }
    }
}
