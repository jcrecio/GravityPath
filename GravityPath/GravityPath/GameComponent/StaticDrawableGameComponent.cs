namespace GravityPath.GameComponent
{
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    public class StaticDrawableGameComponent : DrawableGameComponent
    {
        private readonly Texture2D texture2D;
        private readonly SpriteBatch spriteBatch;
        private readonly Vector2 position;
        private readonly Color color;
        private readonly float rotation = 0;

        public StaticDrawableGameComponent(Game game, SpriteBatch spriteBatch, Texture2D texture2D, Vector2 position) : base(game)
        {
            this.texture2D = texture2D;
            this.spriteBatch = spriteBatch;
            this.position = position;
        }

        public StaticDrawableGameComponent(Game game, SpriteBatch spriteBatch, Texture2D texture2D, Vector2 position, Color color)
            : this(game, spriteBatch, texture2D, position)
        {
            this.color = color;
        }

        public StaticDrawableGameComponent(Game game, SpriteBatch spriteBatch, Texture2D texture2D, Vector2 position, Color color, float rotation)
            : this(game, spriteBatch, texture2D, position, color)
        {
            this.rotation = rotation;
        }

        // Even in static sprites we might change the position depending on viewpont, f.i. backgrounds
        public void Draw(GameTime gameTime, int adjustment)
        {
            this.spriteBatch.Draw(this.texture2D, adjustment == 0 ? this.position : new Vector2(this.position.X, this.position.Y + adjustment), null, this.color, this.rotation, Vector2.Zero, 1, SpriteEffects.None, 1);
            base.Draw(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Draw(this.texture2D, this.position, null, this.color, this.rotation, Vector2.Zero, 1, SpriteEffects.None, 1);
            base.Draw(gameTime);
        }
    }
}