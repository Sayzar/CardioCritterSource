using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CardioCritters.Code.GUI.Meter
{
    public class StatusMeter : Meter
    {

        public StatusMeter(Color barColor)
            : base("Minigame/HeftyHeart/Meter/outerbar", "Minigame/HeftyHeart/Meter/fillbarWhite")
        {
            ChangeFillBarColor(barColor);
        }


        public void Update(GameTime gameTime, float percentage)
        {
            base.Update(gameTime);

            int length = (int)(24 + ((215 - 24) * percentage));
            int height = this.originalDimensions.Height;

            fillRect = new Rectangle(0, 0, length, height);
        }
    }
}