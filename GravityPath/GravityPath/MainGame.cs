namespace GravityPath
{
    using System;
    using System.Collections.Generic;
    using EntityGame;
    using Enumeration;
    using GravityPath.Args;
    using GravityPath.GameComponent;
    using Microsoft.Xna.Framework.Content;
    using Services;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System.Linq;

    public class MainGame : Game
    {
        #region Ship

        private const float AccelerationPropellPlayerRate = 0.056f;
        private const float RotationPropellPlayerRate = 0.036f;
        private readonly Vector2 accelerationDownShip = new Vector2(0, 0.009f);
        private const float TopSpeed = 0.9f;

        #endregion

        #region Graphics

        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        #endregion

        #region Services

        private GeneralContainer generalContainer;
        private LevelManager levelManager;
        private InputManager inputManager;
        private SpriteManager spriteManager;
        private ContentProvider contentProvider;
        private ContentGenerator contentGenerator;

        #endregion

        #region Level related

        private GameState GameState
        {
            get { return this.gameState; }
            set
            {
                this.gameState = value;
                this.StateEvent.Invoke(this, new StateGameEventArgs(value));
            }
        }

        private BasicItem player;
        private int currentLevelNumber;

        private const int HalfScreen = (int)Enumeration.Ship.HalfScreen;
        private float adjustment;

        private string randomMessage = string.Empty;
        private GameState gameState;

        private int displacementMenu = 0;

        #endregion

        #region State

        public event Action<object, StateGameEventArgs> StateEvent;

        #endregion

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            TargetElapsedTime = TimeSpan.FromTicks(333333);

            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("SpriteFont/Kootenay");

            this.GraphicsLoadSettings();
            this.LoadGeneralContainer();

            generalContainer.Register<GraphicsDevice>(GraphicsDevice);
            generalContainer.Register<ContentManager>(Content);

            contentProvider = ContentProvider.GetInstance();
            generalContainer.Register<ContentProvider>(contentProvider);

            inputManager = InputManager.GetInstance(InputType.WindowsPhone);
            generalContainer.Register<InputManager>(inputManager);

            spriteManager = SpriteManager.GetInstance();
            generalContainer.Register<SpriteManager>(spriteManager);
            StateEvent += spriteManager.GameStateChanged;

            contentGenerator = ContentGenerator.GetInstance();
            generalContainer.Register<ContentGenerator>(contentGenerator);

            levelManager = LevelManager.GetInstance();
            generalContainer.Register<LevelManager>(levelManager);

            this.InitializeNewGame();
            this.randomMessage = this.GetRandomMessage();
        }

        private void LoadGeneralContainer()
        {
            generalContainer = GeneralContainer.GetInstance();
            generalContainer.Register<Game>(this);
            generalContainer.Register<SpriteBatch>(spriteBatch);
        }

        private void GraphicsLoadSettings()
        {
            this.graphics.PreferredBackBufferWidth = 480;
            this.graphics.PreferredBackBufferHeight = 800;
            this.graphics.IsFullScreen = true;
            this.graphics.SupportedOrientations = DisplayOrientation.Portrait;
            this.graphics.ApplyChanges();
        }

        private void InitializePlayer()
        {
            this.player = this.contentProvider.GetPlayer();
            this.player.Initialize();

            this.player.Speed = new Vector2(0, 0.05f);
            this.player.Position = new Vector2(230, 0);
        }

        private void InitializeLevelManager()
        {
            // We subscribe to level changes
            levelManager.SubscribeToLevelChange(i => { this.currentLevelNumber = i; });
            levelManager.SubscribeToLevelChange(i => { spriteManager.Planets = levelManager.CurrentLevel.Planets; });

            levelManager.Initialize();
        }

        private void InitializeSpriteManager(GameState gameStateToInitialize)
        {
            switch (gameStateToInitialize)
            {
                case GameState.Level:
                {
                    this.spriteManager.BackgroundComponent = contentProvider.GetBackground(1);

                    if (this.spriteManager.BasicItems.Count == 0)
                    {
                        this.spriteManager.BasicItems.Add(this.player);
                    }
                    else
                    {
                        this.spriteManager.BasicItems[0] = this.player;
                    }

                    this.spriteManager.StaticComponents.Add(this.contentProvider.GetArrowLeft());
                    this.spriteManager.StaticComponents.Add(this.contentProvider.GetArrowRight());
                    this.spriteManager.SubscribeToPlanetsChanged(levelManager);
                    this.spriteManager.EventHorizons = this.contentProvider.GetEventsHorizon().ToList();

                    break;
                }
                case GameState.Menu:
                {
                    this.displacementMenu = 0;
                    this.spriteManager.BackgroundComponent = contentProvider.GetMenuBackground();
                    this.spriteManager.TitleTexture = contentProvider.GetTextToTitle();
                    this.spriteManager.PlayTexture = contentProvider.GetTextToPlay();
                    this.spriteManager.ScoresTexture = contentProvider.GetTextToScore();
                    this.spriteManager.AboutTexture = contentProvider.GetTextToAbout();

                    break;
                }
            }
        }

        private void InitializeNewGame()
        {
            this.InitializePlayer();
            this.InitializeLevelManager();
            this.InitializeSpriteManager(GameState.Menu);
            this.GameState = GameState.Menu;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (GameState)
            {
                case GameState.Level:
                {
                    this.UpdateLevel(gameTime);
                    break;
                }
                case GameState.GameOver:
                {
                    this.UpdateGameOver();
                    break;
                }

                case GameState.Menu:
                {
                    this.UpdateMenu();
                    break;
                }
            }

            base.Update(gameTime);
        }

        private void UpdateGameOver()
        {
            if (!this.inputManager.GetUserTouch().Equals(-1))
            {
                this.player.Status = PlayerStatus.Alive;

                this.InitializeNewGame();
                this.ReestablishParameters();
            }
        }

        private void UpdateMenu()
        {
            var userClick = this.inputManager.GetTouchOption();
            switch (userClick)
            {
                case 1:
                {
                    this.GameState = GameState.Level;
                    break;
                }
                case 2:
                {
                    this.GameState = GameState.Scores;
                    break;
                }
                case 3:
                {
                    this.GameState = GameState.About;
                    break;
                }
            }

            this.InitializeSpriteManager(this.GameState);

        }

        private void UpdateLevel(GameTime gameTime)
        {
            if (this.player.Status.Equals(PlayerStatus.Alive))
            {
                // Updating speed step by step until every operation it's over
                var currentUserSpeed = this.player.Speed;

                // Get the user input - none (-1), left(0) or right(1)
                var userMovement = this.inputManager.GetUserTouch();

                var speedVariation = this.GetSpeedWithUserMovement(userMovement);
                this.SetPlayerRotation(speedVariation);

                // Applying the final force over the ship due to planets
                var resultingForce = this.levelManager.GetForceAtIteration(this.player) + speedVariation;

                // , the ship is always accelerating down because of its engine to the maximum speed
                if (this.player.Speed.Length() < TopSpeed) resultingForce += this.accelerationDownShip;

                var displacement = Vector2.Multiply(this.player.Speed, gameTime.ElapsedGameTime.Milliseconds);
                this.SetPlayerSpeed(currentUserSpeed, resultingForce);

                this.adjustment = 0;

                this.player.Update(gameTime, displacement);

                this.SetAdjustment(displacement);
                this.DetectCollision(gameTime);
            }
            else
            {
                this.player.Update(gameTime, Vector2.Zero);

                if (this.player.ExplosionIndicator <= 0)
                {
                    this.randomMessage = this.GetRandomMessage();
                    this.GameState = GameState.GameOver;
                }
            }
        }

        private void ReestablishParameters()
        {
            levelManager.Levels = contentProvider.GetPlanets(contentProvider.LevelPlanetFiller());
            this.spriteManager.BackgroundComponent = contentProvider.GetBackground(1);
            this.spriteManager.StaticComponents = new List<StaticDrawableGameComponent>
            {
                this.contentProvider.GetArrowLeft(),
                this.contentProvider.GetArrowRight(),
            };

            this.spriteManager.EventHorizons = this.contentProvider.GetEventsHorizon().ToList();
            this.SetProperYCoordinateForPlanets(1);
        }

        private void SetProperYCoordinateForPlanets(int level)
        {
            this.levelManager.SetProperlyYToPlanetsFromLevel(level);
        }

        private void DetectCollision(GameTime gameTime)
        {
            this.DetectBorderCollision();

            var planets = levelManager.CurrentLevel.Planets;
            foreach (var planet in planets)
            {
                planet.Update(gameTime);
                if (player.Intersects(planet, ShapeObject.Circle))
                {
                    this.player.Status = PlayerStatus.Exploding;
                }
            }
        }

        private void DetectBorderCollision()
        {
            if (this.player.Position.X < -10 || this.player.Position.X > 500)
            {
                this.player.Status = PlayerStatus.Exploding;
            }
        }

        private void SetAdjustment(Vector2 displacement)
        {
            if (this.player.Position.Y >= HalfScreen)
            {
                this.adjustment = displacement.Y;
            }

            if (this.player.Position.Y <= 1600)
            {
                this.levelManager.CurrentLevel.Planets.Where(p=>p.Position.Y<=1600).ToList().ForEach(p=>p.IncrementObjectIndex = true);
            }
        }

        private void SetPlayerSpeed(Vector2 currentUserSpeed, Vector2 speedVariation)
        {
            currentUserSpeed += speedVariation;
            this.player.Speed = currentUserSpeed;
        }

        private void SetPlayerRotation(Vector2 speedVariation)
        {
            if (speedVariation.X < 0)
            {
                this.player.Rotation += RotationPropellPlayerRate;
            }
            else if (speedVariation.X > 0)
            {
                this.player.Rotation -= RotationPropellPlayerRate;
            }
        }

        private Vector2 GetSpeedWithUserMovement(int userMovement)
        {
            switch (userMovement)
            {
                case (int)InputTouch.LeftMovement:
                {
                    return new Vector2(-AccelerationPropellPlayerRate, 0);
                }
                case (int)InputTouch.RightMovement:
                {
                    return new Vector2(AccelerationPropellPlayerRate, 0);
                }
                case (int)InputTouch.NeutralMovement:
                {
                    return Vector2.Zero;
                }
                default:
                {
                    return Vector2.Zero;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            switch (GameState)
            {
                case GameState.Level:
                {
                    if (player.Status.Equals(PlayerStatus.Exploding)) adjustment = 0.001f;
                    spriteManager.Draw(gameTime, adjustment, player.Position.Y, player.Speed.Length(), levelManager.CurrentLevel.Score);
                    break;
                }
                case GameState.GameOver:
                {
                    spriteBatch.Begin();
                    spriteBatch.DrawString(spriteFont, this.randomMessage, new Vector2(10, 100), Color.Black );
                    spriteBatch.DrawString(spriteFont, "Pulsa para RESTART.", new Vector2(10, HalfScreen), Color.Black);
                    spriteBatch.End();
                    break;
                }
                case GameState.Menu:
                {
                    spriteManager.Draw(gameTime);
                    break;
                }
            }

            base.Draw(gameTime);
        }

        private string GetRandomMessage()
        {
            Random r = new Random();
            var x = r.Next(0, 3);
            switch (x)
            {
                case 0:
                {
                    return "Wow! gravity can put you in orbit!";
                }
                case 1:
                {
                    return "Take care!";
                }
                case 2:
                {
                    return "Oh man..";
                }
            }
            return "Do it smoothly";
        }
    }
}
