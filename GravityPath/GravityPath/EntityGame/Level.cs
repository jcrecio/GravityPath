namespace GravityPath.EntityGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GravityPath.Args;

    public class Level
    {
        public event Action<object, PlanetUpdatedEventArgs> PlanetsUpdated;

        public int Score { get; set; }

        private int levelNumber;
        private List<Planet> planets;

        public int LevelNumber {
            get { return levelNumber;  }
            private set { levelNumber = value; }
        }

        public List<Planet> Planets
        {
            get { return this.planets; }
            set
            {
                this.planets = value;
                if (this.PlanetsUpdated != null)
                {
                    this.PlanetsUpdated.Invoke(this, new PlanetUpdatedEventArgs(this.planets));
                }
            }
        }

        public int LengthY { get; set; }

        public Level(int levelNumber, IEnumerable<Planet> planets, int lengthY)
        {
            LevelNumber = levelNumber;

            Planets = planets.ToList();

            LengthY = lengthY;

            Score = 0;
        }
    }
}