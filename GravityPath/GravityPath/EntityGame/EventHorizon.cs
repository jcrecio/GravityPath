namespace GravityPath.EntityGame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class EventHorizon : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D texture;
        private Rectangle rectangle;
        private Rectangle endrectangle;

        private int X1 { get; set; }
        private int X2 { get; set; }
        private int Y1 { get; set; }
        private int Y2 { get; set; }

        public EventHorizon(Game game, SpriteBatch spriteBatch, int x1, int y1, int x2, int y2)
            : base(game)
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
            this.spriteBatch = spriteBatch;
            this.rectangle = new Rectangle(X1, Y1, 482, 8);
            this.endrectangle = new Rectangle(X1 + 482, Y1 - 10, 5, 8);
            this.texture = game.Content.Load<Texture2D>("Graphics/Levels/Level1/redline");
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(texture, rectangle, null, Color.LightBlue, 0, Vector2.Zero, SpriteEffects.None, 0);
            // spriteBatch.Draw(texture, new Vector2(), null, Color.White, -120 );
            base.Draw(gameTime);
        }

    }
}