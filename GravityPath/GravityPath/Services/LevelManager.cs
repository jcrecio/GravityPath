using Microsoft.Xna.Framework.Graphics;

namespace GravityPath.Services
{
    using GravityPath.Args;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EntityGame;
    using System;
    using System.Linq;

    public class LevelManager
    {
        private static LevelManager _levelManager;
        private Level currentLevel;

        private readonly ContentProvider contentProvider;
        private readonly ContentGenerator contentGenerator;
        private List<DangerSignal> dangerSignals;

        public event Action<object, DangerSignalEventArgs> DangerSignalEventCompleted;

        public static LevelManager GetInstance()
        {
            return _levelManager ?? (_levelManager = new LevelManager());
        }

        private LevelManager()
        {
            var container = GeneralContainer.GetInstance();

            contentProvider = container.GetServiceInstance<ContentProvider>();
            contentGenerator = container.GetServiceInstance<ContentGenerator>();
        }

        public Dictionary<int, Level> Levels { get; set; }

        public List<DangerSignal> DangerSignals
        {
            get { return this.dangerSignals; }
            set
            {
                this.dangerSignals = value;
                if (this.DangerSignalEventCompleted != null)
                {
                    this.DangerSignalEventCompleted.Invoke(this, new DangerSignalEventArgs(value));
                }
            }
        }

        public Level CurrentLevel
        {
            get { return this.currentLevel; }
            private set
            {
                this.currentLevel = value;
                this.LevelChangeEvent.Invoke(this.currentLevel.LevelNumber);
            }
        }

        private event Action<int> LevelChangeEvent;
        public void SubscribeToLevelChange(Action<int> action)
        {
            this.LevelChangeEvent += action;
        }

        public void SubscribeToPlanetUpdated(Action<object, PlanetUpdatedEventArgs> action)
        {
            this.CurrentLevel.PlanetsUpdated += action;
        }

        public void SubscribeToDangerUpdated(Action<object, DangerSignalEventArgs> action)
        {
            this.DangerSignalEventCompleted += action;
        }

        public Vector2 GetForceAtIteration(BasicItem basicItem)
        {
            var planetsToRemoveOfTheScene = new List<Planet>();
            Vector2 force = Vector2.Zero;

            foreach (var planet in CurrentLevel.Planets)
            {
                var mass = planet.Mass;
                if (mass.Equals(0))
                {
                    planetsToRemoveOfTheScene.Add(planet);
                }

                force += planet.GetForceOverPlanet(basicItem);
            }

            if (planetsToRemoveOfTheScene.Count > 0)
            {
                this.CurrentLevel.Score += planetsToRemoveOfTheScene.Count;
                this.UpdatePlanetsInAction(planetsToRemoveOfTheScene, (int) basicItem.Position.Y);
            }

            return force;
        }

        private void UpdatePlanetsInAction(List<Planet> planetsToRemoveOfTheScene, int posPlayerY)
        {
            var listPlanets = this.CurrentLevel.Planets.Except(planetsToRemoveOfTheScene).ToList();
            var listNewPlanetsToCreate = this.contentGenerator.GeneratePlanets(posPlayerY + 800, 1).ToList();

            var listFinalDangerSignals = this.DangerSignals.Skip(planetsToRemoveOfTheScene.Count).ToList();
            var listNewDangerSignalsToCreate = this.contentGenerator.GenerateDangerSignals(listNewPlanetsToCreate);

            listPlanets.AddRange(listNewPlanetsToCreate.Select(p => this.contentProvider.PlanetFillerToPlanet(p, posPlayerY)));
            listFinalDangerSignals.AddRange(listNewDangerSignalsToCreate.Select(d => this.contentProvider.DangerFillerToDangerSignal(d)));

            this.CurrentLevel.Planets = listPlanets;
            this.DangerSignals = listFinalDangerSignals;
        }

        public void SetProperlyYToPlanetsFromLevel(int level)
        {
            var planets = this.Levels.FirstOrDefault(l => l.Value.LevelNumber.Equals(level)).Value.Planets;
            planets.ForEach(p => p.SetProperlyY());
        }

        public void Initialize()
        {
            var fillers = contentProvider.LevelPlanetFiller();
            var planetFillers = fillers.ToList();

            Levels = contentProvider.GetPlanets(planetFillers);
            DangerSignals = contentProvider.GetDangers(planetFillers).ToList();
            CurrentLevel = Levels.FirstOrDefault().Value;
        }
    }
}
