using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CardioCritters.Code.GUI.Meter
{
    public class HeftyHeartMeter : Meter
    {
        protected Entity icon;
        protected Texture2D[] iconTextures;

        public HeftyHeartMeter()
            : base("Minigame/HeftyHeart/Meter/outerbar", "Minigame/HeftyHeart/Meter/fillbar")
        {
            InitializeIcons(
                new String[] { "Minigame/HeftyHeart/Meter/littleheart1",
                    "Minigame/HeftyHeart/Meter/littleheart2", 
                    "Minigame/HeftyHeart/Meter/littleheart3" }
                );

        }

        public HeftyHeartMeter(String outerbar, String fillbar, String[] icons)
            : base(outerbar, fillbar)
        {
            InitializeIcons(icons);
        }

        public void InitializeIcons(String[] icons)
        {
            icon = new Entity(icons[0]);
            icon.renderIndex = 0.0f;
            icon.SetPosition(new Vector2(base.dimensions.Width / 2, base.dimensions.Height / 10));

            iconTextures = new Texture2D[icons.Length];
            for (int i = 0; i < icons.Length; i++)
            {
                iconTextures[i] = Utility.GetTexture(icons[i]);
            }
        }

        public override void Update(GameTime gameTime, float percentage)
        {
            if (percentage >= 1.0) icon.sprite = iconTextures[2];
            else if (percentage >= 0.5) icon.sprite = iconTextures[1];
            else icon.sprite = iconTextures[0];

            base.Update(gameTime, percentage);
            
            // CALCULATE DIMENSIONS
            // remember that the fill starts at pixel 20 
            // and ends at pixel 215 (with the original dimensions) of the HeftyHeartbar
            int length = (int)(24 + ((215 - 24) * percentage));
            int height = this.originalDimensions.Height;

            fillRect = new Rectangle(0, 0, length, height);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            icon.Draw(sb);
        }

        public override void SetPosition(Vector2 pos)
        {
            icon.SetPosition(pos + new Vector2(base.dimensions.Width/2,base.dimensions.Height/10));
            base.SetPosition(pos);
        }
        
    }
}