namespace GravityPath.GameComponent
{
    using GravityPath.Services;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Componente de juego que implementa IUpdateable.
    /// </summary>
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteManager spriteManager;

        public MainMenu(Game game, SpriteManager spriteManager)
            : base(game)
        {
            this.spriteManager = spriteManager;
        }

        /// <summary>
        /// Permite al componente de juego realizar la inicialización necesaria antes de empezar a
        /// ejecutarse. En este punto puede consultar los servicios necesarios y cargar contenido.
        /// </summary>
        public override void Initialize()
        {
            // TODO: agregue aquí su código de inicialización

            base.Initialize();
        }

        /// <summary>
        /// Permite al componente de juego actualizarse.
        /// </summary>
        /// <param name="gameTime">Proporciona una instantánea de los valores de tiempo.</param>
        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }
    }
}
