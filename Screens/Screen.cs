using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CardioCritters.Screens
{
    public abstract class Screen
    {
        public Texture2D staticBackground;
        public SpriteBatch _sb;
        /// <summary>
        /// This screen is just overlayed over the last one, (ie a hud) so Screen Stack will know to keep updating the screen below this one.
        /// </summary>
        public bool IsOverlay { get; protected set; }

        /// <summary>
        /// Null for no background, otherwise pass in path to a background texture
        /// (TODO: investigate loading backgrounds in beginning and just their texture references here
        /// to minimize in-game loading)
        /// </summary>
        /// <param name="background"></param>
        public Screen(String background)
        {
            if (background != null)
            {
                // adds it if it doesn't exist yet
                Utility.AddNewTexture(background);

                staticBackground = Utility.GetTexture(background);
            }
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch sb)
        {
            _sb = sb;
            if (staticBackground != null)
            {
                //sb.Draw(staticBackground, new Rectangle(0, 0, Utility.ScreenWidth, Utility.ScreenHeight), Color.White);
                _sb.Draw(staticBackground,
                    new Rectangle(0, 0, Utility.ScreenWidth, Utility.ScreenHeight),
                    null,
                    Color.White,
                    0f,
                    new Vector2(0,0),
                    SpriteEffects.None,
                    1f);
            }
        }
    }
}