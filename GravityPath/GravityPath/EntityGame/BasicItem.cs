namespace GravityPath.EntityGame
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using GravityPath.Contract;
    using GravityPath.Enumeration;

    public class BasicItem : DrawableGameComponent, IIntersectableObject
    {
        private const int HalfScreen = (int)Enumeration.Ship.HalfScreen;

        public Vector2 Speed { get; set; }
        public Vector2 Position { get; set; }

        public PlayerStatus Status { get; set; }

        public int ExplosionIndicator { get; private set; }
        private bool growingExplosion = true;

        public Rectangle ObjectArea
        {
            get
            {
                return new Rectangle(
                    (int)(this.Position.X - halfTextureSizeX),
                    (int)(this.Position.Y - halfTextureSizeY), 
                    textureSize.X, 
                    textureSize.Y);
            }
        }

        public float Radius { get; private set; }

        public float Rotation { get; set; }

        private readonly Texture2D texture2D;

        private Point textureGrid;
        private Point textureSize;

        // Store these values makes calculations faster
        private float halfTextureSizeX;
        private float halfTextureSizeY;

        private Point currentTexture;

        private readonly SpriteBatch spriteBatch;

        public BasicItem(Game game, SpriteBatch spriteBatch, Texture2D texture2D, Point textureGrid, Point textureSize)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.texture2D = texture2D;
            this.textureGrid = textureGrid;
            this.textureSize = textureSize;

            this.halfTextureSizeX = textureSize.X/2;
            this.halfTextureSizeY = textureSize.Y/2;

            this.currentTexture = new Point(0, 0);
            this.Status = PlayerStatus.Alive;
            this.ExplosionIndicator = 1;
        }

        public override void Initialize()
        {
            base.Initialize();

            this.Speed = Vector2.Zero;
            this.Position = Vector2.Zero;
            this.Rotation = 0;
        }

        public void Update(GameTime gameTime, Vector2 displacement)
        {
            if (Status.Equals(PlayerStatus.Alive))
            {
                this.SetCurrentTexture();
                this.Position += displacement;

                base.Update(gameTime);
            }
            else
            {
                if (ExplosionIndicator == 201) growingExplosion = false;
                if (growingExplosion) ExplosionIndicator+=10;
                else ExplosionIndicator-=10;
            }
        }

        private void SetCurrentTexture()
        {
            this.currentTexture.X++;
            if (this.currentTexture.X == this.textureGrid.X)
            {
                if (this.currentTexture.Y == this.textureGrid.Y) this.currentTexture = new Point(0, 0);
                else
                {
                    this.currentTexture.X = 0;
                    this.currentTexture.Y++;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.DrawAnimation(0);
            base.Draw(gameTime);
        }

        public void Draw(GameTime gameTime, float adjustment)
        {
            this.DrawAnimation(adjustment);
            base.Draw(gameTime);
        }

        private void DrawAnimation(float adjustment)
        {
            if (Status.Equals(PlayerStatus.Alive))
            {
                this.spriteBatch.Draw(
                    this.texture2D,
                        adjustment.Equals(0) 
                            ? new Vector2(this.Position.X - halfTextureSizeX, this.Position.Y - halfTextureSizeY) 
                            : new Vector2(this.Position.X - halfTextureSizeX, HalfScreen),
                    new Rectangle(this.currentTexture.X * this.textureSize.X, this.currentTexture.Y * this.textureSize.Y,
                        this.textureSize.X, this.textureSize.Y),
                    Color.White,
                    this.Rotation,
                    Vector2.Zero,
                    1,
                    SpriteEffects.None,
                    1
                    );
            }
            else
            {
                var texture = CreateCircle(ExplosionIndicator);

                this.spriteBatch.Draw(
                    texture,
                     adjustment.Equals(0) 
                            ? new Vector2(this.Position.X, this.Position.Y) 
                            : new Vector2(this.Position.X, HalfScreen),
                    null,
                    Color.White,
                    this.Rotation,
                    Vector2.Zero,
                    1,
                    SpriteEffects.None,
                    1
                    );
            }
        }

        Texture2D CreateCircle(int radius)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.Red;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        public bool Intersects(IIntersectableObject intersectableObject, ShapeObject shapeObject)
        {
            switch (shapeObject)
            {
                case ShapeObject.Circle:
                {
                    float radius = intersectableObject.Radius;
                    Vector2 circleCenter = intersectableObject.Position;

                    // Calculate the distance between the circle's center and this closest point
                    float distanceX = circleCenter.X - Position.X;
                    float distanceY = circleCenter.Y - Position.Y;

                    // If the distance is less than the circle's radius, an intersection occurs
                    float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);

                    return distanceSquared < (radius * radius);
                }
            }
            return false;
        }
    }
}
