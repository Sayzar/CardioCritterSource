using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CardioCritters.Code.GUI;
using Microsoft.Xna.Framework.Input.Touch;
using CardioCritters.Code.GUI.Buttons;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace CardioCritters.Screens
{
    public class MainMenu : Screen
    {
        private Button play, credits, options;

        public MainMenu() : 
#if ANDROID
            base("MainMenu/Background")
#endif
#if WINDOWS
            base("MainMenu/background")
#endif
        {
            // Make the buttons
            play = new NewScreenButton("MainMenu/Buttons/newgame", "MainMenu/Buttons/newgame2", "Tamagotchi", false);
            options = new NewScreenButton("MainMenu/Buttons/options", "MainMenu/Buttons/options2", "Options", false);
            credits = new NewScreenButton("MainMenu/Buttons/credits", "MainMenu/Buttons/credits2", "Credits", false);

            // Set the sizes of the buttons
            play.SetSizeRelativeToScreen(0.35f, 0.2f);
            options.SetSizeRelativeToScreen(0.35f, 0.2f);
            credits.SetSizeRelativeToScreen(0.35f, 0.2f);

            // Set the positions
            play.SetPosition(new Vector2(0, Utility.ScreenHeight / 2 - play.dimensions.Height * 3/ 2));
            options.SetPosition(new Vector2(0, Utility.ScreenHeight/2 - options.dimensions.Height/2));
            credits.SetPosition(new Vector2(0, Utility.ScreenHeight / 2 + credits.dimensions.Height / 2));

            play.Move(Utility.ScreenWidth / 10, Utility.ScreenHeight / 10);
            options.Move(Utility.ScreenWidth / 10, Utility.ScreenHeight / 10);
            credits.Move(Utility.ScreenWidth / 10, Utility.ScreenHeight / 10);

            // some loading for background for minigames...
            Utility.AddNewTexture("Minigame/Fallinggrub/darkbgeat");

            // for some reason we have to use the extensions because the .xnb files doesn't work
            String songStr = "Music/raw/theme.mp3";

#if ANDROID
            Utility.AddNewSong(songStr);
            Song song = Utility.GetSong(songStr);
            Utility.musicManager.PlayMusic(song);
            Utility.musicManager.Loop();
#endif

            String soundStr = "Music/raw/button.wav";
            Utility.AddNewSoundEffect(soundStr);
           // se = Utility.GetSoundEffect(soundStr);
           // Utility.musicManager.PlaySound(se);
        }
        SoundEffect se;

        public override void Update(GameTime gameTime)
        {
            Vector2 pos = new Vector2(-1, -1);

            foreach (TouchLocation loc in Utility.inputManager.GetTouches())
                pos = loc.Position;

            play.HandleInput(pos);
            options.HandleInput(pos);
            credits.HandleInput(pos);
        }

        public override void Draw(SpriteBatch sb)
        {
            // Always draw the background screen first
            base.Draw(sb);

            // Draw the buttons
            play.Draw(sb);
            options.Draw(sb);
            credits.Draw(sb);
        }
    }
}