using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CardioCritters.Code.GUI.Meter
{
    public class Meter : Entity
    {
        protected Texture2D fill;
        protected Rectangle fillRect;
        protected Color fillbarColor;

        public Meter(String background, String fill)
            : base(background)
        {
            this.fill = Utility.GetTexture(fill);
            fillRect = new Rectangle(0, 0, 0, this.dimensions.Height);
            this.renderIndex = 0.1f;
            // default color drawn
            fillbarColor = Color.White;
        }

        public void ChangeFillBarColor(Color newColor)
        {
            fillbarColor = newColor;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            // what this does is it stretches or shrinks the image instead of drawing the specific spot
            //fillRect.X = this.dimensions.X;
            //fillRect.Y = this.dimensions.Y;
            //sb.Draw(fill, fillRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);

            Rectangle destRect = new Rectangle( this.dimensions.X, this.dimensions.Y,
                (int) (fillRect.Width * GetCurrentScale().X),
                (int) (fillRect.Height * GetCurrentScale().Y));

            sb.Draw(fill, destRect, fillRect, fillbarColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
        }

        public virtual void Update(GameTime gameTime, float percentage)
        {
            // calculating  the WHOLE TEXTURE would mean that you're including the transparent stuff
            // fillRect = new Rectangle(0, 0, (int)(this.dimensions.Width * percentage), this.dimensions.Height);

            //---//

            // should normally done like this however ONLY if the entire texture of the fillbar texture 
            // is exactly the same dimension as the outerbar texture and the entire fillbar texture has no transparency
            int length = (int)(this.originalDimensions.Width * percentage);
            int height = this.originalDimensions.Height;

            fillRect = new Rectangle(0, 0, length, height);

            base.Update(gameTime);
        }
    }
}