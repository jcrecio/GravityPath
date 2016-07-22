namespace GravityPath.EntityGame
{
    using System.Globalization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class DangerSignal: DrawableGameComponent
    {
        public int X0 { get; set; }
        public int X1 { get; set; }
        public int CenterX { get; set; }
        public float RangeBottomY { get; private set; }
        public float RangeTopY { get; private set; }

        private readonly SpriteBatch spriteBatch;

        private readonly Texture2D texture;
        private readonly Texture2D radar;
        private readonly SpriteFont font;

        public DangerSignal(Game game, SpriteBatch spriteBatch, float x0, float x1, float centerX, float rangeTopY, float rangeBottomY)
            : base(game)
        {
            this.X0 = (int)x0;
            this.X1 = (int)x1;

            this.CenterX = (int)centerX - 25;

            this.RangeBottomY = rangeBottomY;
            this.RangeTopY = rangeTopY;

            this.texture= game.Content.Load<Texture2D>("Graphics/danger");
            this.radar = game.Content.Load<Texture2D>("Graphics/Levels/Level1/redline");
            this.font = game.Content.Load<SpriteFont>("SpriteFont/ScoreFont");

            this.spriteBatch = spriteBatch;
        }

        public void Draw(GameTime gameTime, int priority, float distance)
        {
            Color dangerColor;
            switch (priority)
            {
                case 0:
                {
                    dangerColor = Color.Red;
                    break;
                }
                case 1:
                {
                    dangerColor = Color.LightSalmon;
                    break;
                }
                case 2:
                {
                    dangerColor = Color.LightYellow;
                    break;
                }
                case 3:
                {
                    dangerColor = Color.Yellow;
                    break;
                }
                case 5:
                {
                    dangerColor = Color.YellowGreen;
                    break;
                }
                case 6:
                {
                    dangerColor = Color.Green;
                    break;
                }
                default:
                {
                    dangerColor = Color.White;
                    break;
                }
            }
            this.spriteBatch.DrawString(this.font, distance.ToString(CultureInfo.InvariantCulture), new Vector2(this.X0, 600), dangerColor);
            this.spriteBatch.Draw(this.radar, new Rectangle(this.X0, 635, this.X1-this.X0, 5), dangerColor);
            this.spriteBatch.Draw(this.texture, new Rectangle(this.CenterX, 650, 50, 50), dangerColor);
            base.Draw(gameTime);
        }
    }
}