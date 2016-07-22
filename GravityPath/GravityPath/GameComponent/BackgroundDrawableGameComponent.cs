namespace GravityPath.GameComponent
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class BackgroundDrawableGameComponent : DrawableGameComponent
    {
        private readonly Texture2D texture2D;
        private readonly SpriteBatch spriteBatch;
        private readonly Vector2 position;
        private float positionPre;
        private float positionPost;
        private readonly Color color = Color.White;
        private readonly float rotation = 0;
        private float index = 0;
        private const float SpeedSensationCoefficient = 10;

        public BackgroundDrawableGameComponent(Game game, SpriteBatch spriteBatch, Texture2D texture2D, Vector2 position)
            : base(game)
        {
            this.texture2D = texture2D;
            this.spriteBatch = spriteBatch;
            this.position = position;
            this.positionPre = position.Y - 800;
            this.positionPost = position.Y + 800;
        }

        public BackgroundDrawableGameComponent(Game game, SpriteBatch spriteBatch, Texture2D texture2D, Vector2 position,
            Color color)
            : this(game, spriteBatch, texture2D, position)
        {
            this.color = color;
        }

        public BackgroundDrawableGameComponent(Game game, SpriteBatch spriteBatch, Texture2D texture2D, Vector2 position,
            Color color, float rotation)
            : this(game, spriteBatch, texture2D, position, color)
        {
            this.rotation = rotation;
        }

        public void Draw(GameTime gameTime, float adjustment)
        {
            if (adjustment.Equals(0))
            {
                this.spriteBatch.Draw(this.texture2D, this.position, null, this.color, this.rotation, Vector2.Zero, 1, SpriteEffects.None, 1);
            }
            else
            {
                this.index += adjustment;
                if (this.index <= -800 || this.index >= 800)
                {
                    this.index = 0;
                }
                this.spriteBatch.Draw(this.texture2D, new Vector2(this.position.X, this.position.Y - this.index), null, this.color, this.rotation, Vector2.Zero, 1, SpriteEffects.None, 1);
                this.spriteBatch.Draw(this.texture2D, new Vector2(this.position.X, this.positionPre - this.index), null, this.color, this.rotation, Vector2.Zero, 1, SpriteEffects.None, 1);
                this.spriteBatch.Draw(this.texture2D, new Vector2(this.position.X, this.positionPost - this.index), null, this.color, this.rotation, Vector2.Zero, 1, SpriteEffects.None, 1);
            }

            base.Draw(gameTime);
        }


    }
}