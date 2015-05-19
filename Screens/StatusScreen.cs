using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using CardioCritters;
using CardioCritters.Code.GUI;
using CardioCritters.Code.GUI.Meter;
using CardioCritters.Code.GUI.Buttons;

namespace CardioCritters.Screens
{
    public class StatusScreen : Screen
    {
        private Button back;

        StatusMeter experience, fullness, calmness, energy;

        float experienceAmt, fullnessAmt, calmnessAmt, energyAmt;

        public StatusScreen() :
            base("MainMenu/Background1")
        {
            back = new NewScreenButton("MainMenu/Buttons/continue", "MainMenu/Buttons/continue2", "Exit", true);
            back.SetSizeRelativeToScreen(0.35f, 0.2f);
            back.SetPosition(new Vector2(Utility.ScreenWidth - back.dimensions.Width, Utility.ScreenHeight - back.dimensions.Height));

            experience = new StatusMeter(Color.Blue);
            fullness = new StatusMeter(Color.Yellow);
            calmness = new StatusMeter(Color.Green);
            energy = new StatusMeter(Color.Yellow);

            // positioning and stuff
            experience.SetSizeRelativeToScreen(0.5f, 0.2f);
            fullness.SetSizeRelativeToScreen(0.2f, 0.2f);
            calmness.SetSizeRelativeToScreen(0.2f, 0.2f);
            energy.SetSizeRelativeToScreen(0.2f, 0.2f);

            experience.SetPosition(0, 0);
            fullness.SetPosition(0, experience.dimensions.Bottom);
            calmness.SetPosition(0, fullness.dimensions.Bottom);
            energy.SetPosition(0, calmness.dimensions.Bottom);

            // TODO: loading stuff
            SaveGame fileToLoad = FileManager.Load();
            experienceAmt = fileToLoad.experience;
            fullnessAmt = fileToLoad.fullness;
            calmnessAmt = fileToLoad.calmness;
            energyAmt = fileToLoad.energy;
        }

        public override void Update(GameTime gameTime)
        {
            experience.Update(gameTime, experienceAmt / 100);
            fullness.Update(gameTime, fullnessAmt / 100);
            calmness.Update(gameTime, calmnessAmt / 100);
            energy.Update(gameTime, energyAmt / 100);

            Vector2 pos = Vector2.Zero;
            foreach (TouchLocation loc in Utility.inputManager.GetTouches())
                pos = loc.Position;

            back.Update(gameTime);
            back.HandleInput(pos);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            back.Draw(sb);

            experience.Draw(sb);
            fullness.Draw(sb);
            calmness.Draw(sb);
            energy.Draw(sb);
        }
    }
}