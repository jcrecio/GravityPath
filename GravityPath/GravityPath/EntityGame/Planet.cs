using GravityPath.Contract;
using GravityPath.Enumeration;

namespace GravityPath.EntityGame
{
    using System.Diagnostics;
    using Microsoft.Xna.Framework;
    using System;
    using Microsoft.Xna.Framework.Graphics;

    public class Planet: DrawableGameComponent, IIntersectableObject
    {
        private readonly SpriteBatch spriteBatch;
        private readonly Texture2D texture2D;
        private const int HalfScreen = (int) Enumeration.Ship.HalfScreen;

        private float adjustmentWhenCreate;
        private int MassColor;

        public bool IncrementObjectIndex { get; set; }

        private int RelativePositionPlayer
        {
            get { return this.relativePositionPlayer; }
            set
            {
                this.relativePositionPlayer = value;
            }
        }

        public float Radius { get; set; }

        private float index;
        public float Index
        {
            get { return this.index; }
            set
            {
                this.index = value;
            }
        }

        private Vector2 position;

        public bool Intersects(IIntersectableObject intersectableObject, ShapeObject shapeObject)
        {
            return false; //TODO
        }

        public Rectangle ObjectArea { get; set; }

        public Planet(Game game, SpriteBatch spriteBatch, Texture2D texture2D, Point dimensions, int relativePositionPlayer) : base(game)
        {
            this.texture2D = texture2D;
            this.spriteBatch = spriteBatch;
            this.Dimensions = dimensions;
            this.RelativePositionPlayer = relativePositionPlayer;
        }

        private Vector2 drawablePosition;

        public Vector2 Position // Center of the planet
        {
            get { return this.position; }
            set
            {
                this.position = value;
                this.Radius = this.Dimensions.X / 2f;
                Index = this.position.Y - this.Radius - this.relativePositionPlayer + HalfScreen;
                this.drawablePosition = new Vector2(this.position.X - this.Radius, this.position.Y - this.Radius);
            }
        }

        public void SetProperlyY()
        {
            //  this.position.Y += this.Position.Y - 200;
            // better using this one:
            if(this.Index > 0) this.Position = new Vector2(this.Position.X, this.Position.Y + 1000);
        }

        // What this really represents is the mass multiplication between the planet itself and the ship, we avoid many calculations as ship's mass is by
        // convention m=1
        private float mass;
        private int relativePositionPlayer;

        public float Mass
        {
            get { return this.mass; }
            set
            {
                this.mass = value;
                this.MassColor = 255; //(new Random().Next(0,255));
            }
        }

        public Point Dimensions { get; private set; }

        public Vector2 GetForceOverPlanet(BasicItem basicItem)
        {
            var forceDirectionUnitary = Vector2.Subtract(Position, basicItem.Position);

            if (forceDirectionUnitary.Y < -700)
            {
                Mass = 0;
                return Vector2.Zero;
            }

            forceDirectionUnitary.Normalize();

            var distance = Vector2.Distance(Position, basicItem.Position);
            if (distance < 90) // Maximum force to apply is to 90 as the calculation between something less than 0.0000....1 is pretty heavy
            {
                distance = 90;
            }
            var distanceSquare = Math.Pow(distance, 2);

            var forceModule = Mass/distanceSquare;

            return Vector2.Multiply(forceDirectionUnitary, (float) forceModule);
        }

        public override void Update(GameTime gameTime)
        {
            this.ObjectArea = this.IncrementObjectIndex
                ? new Rectangle((int)this.drawablePosition.X, (int)(this.index) - HalfScreen, this.Dimensions.X, this.Dimensions.Y) 
                : new Rectangle((int)this.drawablePosition.X, (int)(this.index), this.Dimensions.X, this.Dimensions.Y);

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, float adjustment)
        {
            if (adjustment > 0)
            {
                Index -= adjustment;
            }

            //this.spriteBatch.Draw(this.texture2D, new Vector2(this.drawablePosition.X, index), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            this.spriteBatch.Draw(this.texture2D,
                ObjectArea,
                Color.White);

            base.Draw(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Draw(this.texture2D, drawablePosition, null, new Color(255, 255, MassColor), 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            base.Draw(gameTime);
        }
    }
}