using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CardioCritters
{
    public class Entity
    {
        public Texture2D sprite;
        Vector2 currentScale;
        public Rectangle dimensions, originalDimensions;
        public float opacity;
        public float rotation;
        public int minX, maxX, minY, maxY;
        private bool isBound;
        public SpriteEffects spriteEffect;
        public float renderIndex;
        public bool hitBoundary;

        public Entity(String spriteFilepath)
        {
            // adds the texture if it doesn't exist yet
            Utility.AddNewTexture(spriteFilepath);

            this.sprite = Utility.GetTexture(spriteFilepath);
            this.originalDimensions = this.dimensions = new Rectangle(0, 0, sprite.Bounds.Width, sprite.Bounds.Height);
            currentScale = new Vector2(1, 1);
            opacity = 1f;
            rotation = 0f;
            spriteEffect = SpriteEffects.None;
            isBound = false;
            renderIndex = 0f;
            hitBoundary = false;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(sprite,
                   dimensions,
                   null,
                   Color.White * opacity,
                   rotation,
                   Vector2.Zero,
                   spriteEffect,
                   renderIndex);

            // Draw the bounding box
            if (Utility.showBoundingBoxes)
                sb.Draw(Utility.GetTexture("Misc/boundingBox"), dimensions, Color.Red);
        }

        public void SetSize(int width, int height)
        {
            dimensions = new Rectangle(dimensions.X, dimensions.Y, width, height);
        }

        public void SetPosition(int x, int y)
        {
            dimensions.X = x;
            dimensions.Y = y;
        }

        public void SetCenterPosition(int x, int y)
        {
            dimensions.X = x - dimensions.Width / 2;
            dimensions.Y = y - dimensions.Height / 2;
        }

        public void SetRotationDegrees(float degrees)
        {
            rotation = MathHelper.ToRadians(degrees);
        }

        public void SetRotationRadians(float rad)
        {
            rotation = rad;
        }

        public float GetRotationDegrees()
        {
            return MathHelper.ToDegrees(rotation);
        }

        public float GetRotationRadians()
        {
            return rotation;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(dimensions.X, dimensions.Y);
        }

        public Vector2 GetCenter()
        {
            return new Vector2(dimensions.Center.X, dimensions.Center.Y);
            //return new Vector2(dimensions.X + dimensions.Width / 2, dimensions.Y + dimensions.Height / 2);
        }

        // Makes it so that the entity is bounded by this
        public void SetBounds(int minX, int maxX, int minY, int maxY, bool useBounds)
        {
            this.maxX = maxX;
            this.minX = minX;
            this.maxY = maxY;
            this.minY = minY;
            this.isBound = useBounds;
        }

        // Sets the bounds to the standard
        public void SetBoundsToScreen()
        {
            SetBounds(0, (int)Utility.ScreenWidth, 0, (int)Utility.ScreenHeight, true);
        }

        // Returns whether or not the point exists within the space of the entity
        public bool Intersects(Vector2 position)
        {
            return InBetween(position.X, this.dimensions.X, this.dimensions.X + this.dimensions.Width)
                    && InBetween(position.Y, this.dimensions.Y, this.dimensions.Y + this.dimensions.Height);
        }

        // Given an entity, this returns wether or not this entity intersects with that one
        public bool IntersectsEntity(Entity e)
        {

            bool xOverlap = InBetween(this.dimensions.X, e.dimensions.X, e.dimensions.X + e.dimensions.Width) ||
                    InBetween(e.dimensions.X, this.dimensions.X, this.dimensions.X + this.dimensions.Width);

            bool yOverlap = InBetween(this.dimensions.Y, e.dimensions.Y, e.dimensions.Y + e.dimensions.Height) ||
                    InBetween(e.dimensions.Y, this.dimensions.Y, this.dimensions.Y + this.dimensions.Height);

            return xOverlap && yOverlap;
        }

        public bool InBetween(float toTest, float min, float max)
        {
            return toTest <= max && toTest >= min;
        }

        // Sets the position and sprite's position for this entity
        public virtual void SetPosition(Vector2 point)
        {
            dimensions.X = (int) point.X;
            dimensions.Y = (int) point.Y;
        }

        // For games where there is little physics, this method can save a lot of time with easy movements
        public void Move(float xDelta, float yDelta)
        {
            SetPosition(new Vector2(this.dimensions.X + xDelta, this.dimensions.Y + yDelta));
        }

        // Moves the entity towards the target position by the given amount
        // This splits it up using basic trig
        public void MoveTo(Vector2 position, int speed)
        {
            float Ex = (position.X - this.GetCenter().X);
            // round off x so there's no division by 0
            Ex = Ex == 0 ? 0.000001f : Ex;

            float angleInDegrees = (float)Math.Atan((position.Y - this.GetCenter().Y) / Ex);
            int x = (int)(Math.Cos(angleInDegrees) * speed);
            int y = (int)(Math.Sin(angleInDegrees) * speed);

            if (position.X > this.GetCenter().X) Move(x, y);
            else Move(-x, -y);
        }

        // Given 2 floats from 0.0 - 1.0, this will resize the sprite and dimensions to 
        // that ratio of the current screen size
        public void SetSizeRelativeToScreen(float width, float height)
        {
            float scaleX, scaleY;
            scaleX = width * Utility.ScreenWidth;
            scaleY = height * Utility.ScreenHeight;

            this.dimensions.Width = (int)scaleX;
            this.dimensions.Height = (int)scaleY;

            this.currentScale = new Vector2(scaleX / originalDimensions.Width,
                scaleY / originalDimensions.Height);
        }

        // Sets the scale of the entity (overrides the current scale)
        public void SetScale(float scaleX, float scaleY)
        {
            this.dimensions.Width = (int)(this.originalDimensions.Width * scaleX);
            this.dimensions.Height = (int)(this.originalDimensions.Height * scaleY);
            this.currentScale = new Vector2(scaleX, scaleY);
        }

        public void SetScale(Vector2 toScale)
        {
            SetScale(toScale.X, toScale.Y);
        }

        // Scales the entity off it's curent scale (piggyback's the current scale)
        public void Scale(float scaleX, float scaleY)
        {
            this.dimensions.Width = (int)(this.dimensions.Width * scaleX);
            this.dimensions.Height = (int)(this.dimensions.Height * scaleY);
            this.currentScale *= new Vector2(scaleX, scaleY);
        }

        public void Scale(Vector2 toScale)
        {
            Scale(toScale.X, toScale.Y);
        }

        public Vector2 GetCurrentScale()
        {
            return this.currentScale;
        }

        // Forces the bounds on the entity
        private void ForceBounds()
        {
            hitBoundary = false;

            if (dimensions.X <= minX)
            {
                hitBoundary = true;
                dimensions.X = minX;
            }
            if (dimensions.Y <= minY)
            {
                hitBoundary = true;
                dimensions.Y = minY;
            }
            if (dimensions.X >= maxX - dimensions.Width)
            {
                hitBoundary = true;
                dimensions.X = maxX - dimensions.Width;
            }
            if (dimensions.Y >= maxY - dimensions.Height)
            {
                hitBoundary = true;
                dimensions.Y = maxY - dimensions.Height;
            }
        }

        public void SetToCenterOfScreen()
        {
            this.SetPosition(new Vector2(Utility.ScreenWidth / 2 - dimensions.Width/2, Utility.ScreenHeight / 2 - dimensions.Height/2));
        }

        public virtual void Update(GameTime gameTime)
        {
            // Forces the entity to be in the range 
            if (isBound)
                ForceBounds();
        }
    }
}