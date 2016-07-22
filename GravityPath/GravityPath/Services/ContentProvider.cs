using System.Collections.Generic;
using GravityPath.GameComponent;

namespace GravityPath.Services
{
    using System;
    using System.Linq;
    using EntityGame;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ContentProvider
    {
        private static ContentProvider _contentProvider;
        
        private Game game;
        private SpriteBatch spriteBatch;
        private Texture2D planetDefaultTexture;

        public static ContentProvider GetInstance()
        {
            return _contentProvider ?? (_contentProvider = new ContentProvider());
        }

        private ContentProvider()
        {
            GeneralContainer generalContainer = GeneralContainer.GetInstance();

            this.game = generalContainer.GetServiceInstance<Game>();
            this.spriteBatch = generalContainer.GetServiceInstance<SpriteBatch>();

            planetDefaultTexture = this.game.Content.Load<Texture2D>("Graphics/planet");
        }

        public BasicItem GetPlayer()
        {
            var texture2D = this.game.Content.Load<Texture2D>("Graphics/ship");
            Point textureGrid = new Point(4, 0);
            Point textureSize = new Point(25, 40);
            return new BasicItem(this.game, this.spriteBatch, texture2D, textureGrid, textureSize);
        }

        public BackgroundDrawableGameComponent GetBackground(int level)
        {
            return new BackgroundDrawableGameComponent(
                this.game,
                this.spriteBatch,
                this.game.Content.Load<Texture2D>(string.Concat("Graphics/Levels/Level", level, "/background")),
                Vector2.Zero);
        }

        public StaticDrawableGameComponent GetArrowLeft()
        {
            return new StaticDrawableGameComponent(
                this.game,
                this.spriteBatch,
                this.game.Content.Load<Texture2D>("Graphics/arrow"),
                new Vector2(120, 800),
                new Color(255, 255, 255) * 0.2f,
                (float)Math.PI);
        }

        public StaticDrawableGameComponent GetArrowRight()
        {
            return new StaticDrawableGameComponent(
                this.game,
                this.spriteBatch,
                this.game.Content.Load<Texture2D>("Graphics/arrow"),
                new Vector2(360, 0),
                new Color(255, 255, 255) * 0.2f);
        }

        public Dictionary<int, Level> GetPlanets(IEnumerable<PlanetFiller> fillers)
        {
            var planetsLevel1 = fillers.Select(filler => this.PlanetFillerToPlanet(filler, 0)).ToList();

            return new Dictionary<int, Level>
            {
                {
                    1, new Level(1, planetsLevel1, 2300)
                }
            };
        }

        public Planet PlanetFillerToPlanet(PlanetFiller planetFiller, int posPlayerY)
        {
            return new Planet(game, spriteBatch, planetDefaultTexture, new Point(planetFiller.DimensionX, planetFiller.DimensionY), posPlayerY)
            {
                Mass = planetFiller.Mass,
                Position = new Vector2(planetFiller.PositionX, planetFiller.PositionY)
            };
        }

        public DangerSignal DangerFillerToDangerSignal(DangerFiller dangerFiller)
        {
            return new DangerSignal(game, spriteBatch, dangerFiller.X0, dangerFiller.X1, dangerFiller.CenterX, dangerFiller.RangeTop, dangerFiller.RangeBottom);
        }

        public IEnumerable<PlanetFiller> LevelPlanetFiller()
        {
            GeneralContainer generalContainer = GeneralContainer.GetInstance();
            var contentGenerator = generalContainer.GetServiceInstance<ContentGenerator>();

            var planets = contentGenerator.GeneratePlanets(700, 3);
            return planets;
        }

        public IEnumerable<DangerFiller> DangerSignals(IEnumerable<PlanetFiller> planetFillers)
        {
            GeneralContainer generalContainer = GeneralContainer.GetInstance();
            var contentGenerator = generalContainer.GetServiceInstance<ContentGenerator>();

            var dangers = contentGenerator.GenerateDangerSignals(planetFillers);
            return dangers;
        }

        public IEnumerable<DangerSignal> GetDangers(List<PlanetFiller> fillers)
        {
            var dangersFillers = this.DangerSignals(fillers);

            return dangersFillers.Select(p => this.DangerFillerToDangerSignal(p));
        }

        public IEnumerable<EventHorizon> GetEventsHorizon()
        {
            return new List<EventHorizon>() { new EventHorizon(game, spriteBatch, 0, 700, 480, 5) };
        }

        public SpriteFont GetFont()
        {
            return game.Content.Load<SpriteFont>("SpriteFont/Courier New");
        }

        public Texture2D GetTextToTitle()
        {
            return game.Content.Load<Texture2D>("Graphics/title");
        }

        public Texture2D GetTextToPlay()
        {
            return game.Content.Load<Texture2D>("Graphics/play");
        }

        public Texture2D GetTextToScore()
        {
            return game.Content.Load<Texture2D>("Graphics/scores");
        }

        public Texture2D GetTextToAbout()
        {
            return game.Content.Load<Texture2D>("Graphics/about");
        }

        public BackgroundDrawableGameComponent GetMenuBackground()
        {
            return new BackgroundDrawableGameComponent(
                this.game,
                this.spriteBatch,
                this.game.Content.Load<Texture2D>("Graphics/Menu/background"),
                Vector2.Zero);
        }
    }
}