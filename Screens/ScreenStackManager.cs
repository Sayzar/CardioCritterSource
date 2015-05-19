using System;
using System.Collections.Generic;
using System.Text;
using CardioCritters.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CardioCritters
{
    public static class ScreenStackManager
    {
        //Even though it's internally an array, treat it as a stack.
        public static Screen[] screenStack = new Screen[5]; //Initialized to max num screens expected, up if needed;
        public static int screenTop;

        public static Screen currentScreen { get { return screenStack[screenTop]; } }
        public static void AddScreen(Screen screen)
        {
            screenTop++;
            screenStack[screenTop] = screen;
        }

        public static void ReplaceScreen(Screen screen)
        {
            if (screenTop != 0)
            {
                screenStack[screenTop] = null;
                screenTop--;
            }
            AddScreen(screen);
        }

        public static void PopScene()
        {
            //TODO: Implement transition system? IE instead of immediately popping we can
            //perhaps apply an alpha fade to the top screen and fade it before quietly removing.

            if (screenTop == 1) return; //Guess we always need at least 1 active screen

            screenStack[screenTop] = null;
            screenTop--;
        }

        public static void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
            if (currentScreen.IsOverlay)
            {
                int index = screenTop - 1;
                Screen underScreen = screenStack[index];
                while (index > 0 && screenStack[index].IsOverlay)
                {
                    //Update all overlays
                    underScreen.Update(gameTime);
                    index--;
                    underScreen = screenStack[index];
                }
                underScreen.Update(gameTime); //Don't forget to update the non-overlay one.
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (currentScreen.IsOverlay)
            {
                int index = screenTop - 1;
                while (index > 0 && screenStack[index].IsOverlay)
                {
                    //find the bottom most overlay, so we can draw from bottom up
                    index--;
                }
                while (index < screenTop)
                {
                    Screen underScreen = screenStack[index];
                    underScreen.Draw(spriteBatch);
                    index++;
                }
            }
            currentScreen.Draw(spriteBatch);
        }
    }
}
