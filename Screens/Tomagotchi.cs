using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardioCritters.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CardioCritters.Code.Screens
{
    public class Tomagotchi : Screen
    {
        private String toPrint = "";
        private Entity background;

        public Tomagotchi() : base(null)
        {
            background = new Entity("Tomagotchi/background");
            background.SetSizeRelativeToScreen(2f, 1f);
            background.SetPosition(-Utility.ScreenWidth, 0);
            background.SetBounds(-Utility.ScreenWidth, Utility.ScreenWidth*2, 0, Utility.ScreenHeight, true);
        }

        private void MoveWorld(float deltaX)
        {
            background.Move(deltaX, 0);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Update all the entities
            

            // Update all the buttons

            // On a drag, we must move the world!
            if (Utility.inputManager.GetGesture(GestureType.HorizontalDrag))
            {
                TouchLocation outTouch;
                Utility.inputManager.GetTouches()[0].TryGetPreviousLocation(out outTouch);
                float deltaX = outTouch.Position.X - Utility.inputManager.GetTouches()[0].Position.X;
                MoveWorld(deltaX);          
            }

            // Lastly, update the background
            background.Update();
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            // Draw the background entity
            background.Draw(sb);
            
            // Draw all the buttons
            
            // Draw the heart

            sb.DrawString(Game1.font, toPrint, Vector2.Zero, Color.Black);
        }
    }
}