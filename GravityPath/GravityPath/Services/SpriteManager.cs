namespace GravityPath.Services
{
    using System.Globalization;
    using EntityGame;
    using GravityPath.Args;
    using GravityPath.Enumeration;
    using GravityPath.GameComponent;

    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpriteManager
    {
        private static SpriteManager _spriteManager;

        private readonly SpriteBatch spriteBatchRef;

        public BackgroundDrawableGameComponent BackgroundComponent { get; set; }
        public List<StaticDrawableGameComponent> StaticComponents { get; set; }
        public List<BasicItem> BasicItems { get; set; }
        public List<Planet> Planets { get; set; }
        public List<EventHorizon> EventHorizons { get; set; }

        public Texture2D TitleTexture { get; set; }
        public Texture2D PlayTexture { get; set; }
        public Texture2D ScoresTexture { get; set; }
        public Texture2D AboutTexture { get; set; }


        public List<DangerSignal> DangerSignals
        {
            get { return this.dangerSignals; }
            set
            {
                this.dangerSignals = value;
            }
        }

        public SpriteFont spriteFont;
        private List<DangerSignal> dangerSignals;

        public static SpriteManager GetInstance()
        {
            var manager = _spriteManager ?? (_spriteManager = new SpriteManager());

            return manager;
        }

        private GameState gameState;

        private SpriteManager()
        {
            var generalContainer = GeneralContainer.GetInstance();

            this.spriteBatchRef = generalContainer.GetServiceInstance<SpriteBatch>();

            var contentProvider = generalContainer.GetServiceInstance<ContentProvider>();

            this.spriteFont = contentProvider.GetFont();

            this.StaticComponents = new List<StaticDrawableGameComponent>();
            this.BasicItems = new List<BasicItem>();
            this.Planets = new List<Planet>();
            this.EventHorizons = new List<EventHorizon>();
        }

        public void Draw(GameTime gameTime)
        {
            this.spriteBatchRef.Begin();
            this.DrawMenu(gameTime);
            this.spriteBatchRef.End();
        }

        public void Draw(GameTime gameTime, float adjustment, float y, float currentSpeed, int score)
        {
            this.spriteBatchRef.Begin();

            switch (gameState)
            {
                case GameState.Menu:
                {
                    this.DrawMenu(gameTime);
                    break;
                }
                case GameState.Level:
                {
                    this.DrawLevel(gameTime, adjustment, y, currentSpeed, score);
                    break;
                }
            }

            this.spriteBatchRef.End();
        }

        public void DrawMenu(GameTime gameTime)
        {
            this.BackgroundComponent.Draw(gameTime, -1);
            this.spriteBatchRef.Draw(this.TitleTexture, new Vector2(100, 140), Color.White);
            this.spriteBatchRef.Draw(this.PlayTexture, new Vector2(135, 245), Color.White);
            this.spriteBatchRef.Draw(this.ScoresTexture, new Vector2(105, 365), Color.White);
            this.spriteBatchRef.Draw(this.AboutTexture, new Vector2(115, 485), Color.White);
        }

        private void DrawLevel(GameTime gameTime, float adjustment, float y, float currentSpeed, int score)
        {
            this.BackgroundComponent.Draw(gameTime, adjustment);
            this.StaticComponents.ForEach(c => c.Draw(gameTime));
            this.BasicItems.ForEach(c => c.Draw(gameTime, adjustment));
            this.EventHorizons.ForEach(c => c.Draw(gameTime));

            int priorityDanger = 0;
            for (int i = 0; i < this.Planets.Count; i++)
            {
                this.Planets[i].Draw(gameTime, adjustment);
                if (priorityDanger < 6 && this.DangerSignals != null && this.DangerSignals[i] != null)
                {
                    var danger = this.DangerSignals[i];

                    if (((danger.X0 >= 70 && danger.X0 <= 400) || (danger.X1 >= 70 && danger.X1 <= 400))
                        && this.Planets[i].Position.Y > y)
                    {
                        this.DangerSignals[i].Draw(gameTime, priorityDanger, (int) (this.DangerSignals[i].RangeBottomY - y));
                        priorityDanger++;
                    }
                }
            }

            this.spriteBatchRef.DrawString(this.spriteFont, score.ToString(CultureInfo.InvariantCulture), new Vector2(400, 25),
                Color.White);
        }

        public void SubscribeToPlanetsChanged(LevelManager levelManager)
        {
            levelManager.SubscribeToPlanetUpdated((s, e) =>
            {
                this.Planets = e.PlanetsUpdated;
            });

            levelManager.SubscribeToDangerUpdated((s, e) =>
            {
                this.DangerSignals = e.DangerSignals;
            });
        }

        public void GameStateChanged(object sender, StateGameEventArgs args)
        {
            this.gameState = args.GameState;
        }
    }
}