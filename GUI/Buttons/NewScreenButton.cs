using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardioCritters.Screens;
using CardioCritters.Code.Screens;
using CardioCritters.Code.Minigames;

namespace CardioCritters.Code.GUI.Buttons
{
    public class NewScreenButton : Button
    {
        private String newScreen;
        private bool replace;

        public NewScreenButton(String nonPressed, String pressed, String newScreen, bool replace)
            : base(nonPressed, pressed)
        {
            this.newScreen = newScreen;
            this.replace = replace;
        }

        public override void OnPress()
        {
            Screen toSwitch = null;

            switch (newScreen)
            {
                case "MainMenu":
                    toSwitch = new MainMenu();
                    break;
                case "Options":
                    break;
                case "Credits":
                    break;
                case "Tamagotchi":
					toSwitch = new Tamagotchi();
                    break;

                    /// ALWAYS, ALWAYS, ALWAYS have at least SIX (6) names/locked variables. The screen
                    /// is known to have at least 6 games per screen
                case "Minigames_Food":
                    toSwitch = new MinigameSelection(MinigameSelection.MinigameType.FULLNESS,
                                    new String[] { "FoodForThought", "", "", "", "", "", "", "" }, 
                                    new bool[]{false, true, true, true, true, true, true, true});
                    break;
                case "Minigames_Health":
                    toSwitch = new MinigameSelection(MinigameSelection.MinigameType.INTERNAL,
                                new String[] { "VeinFlyer", "", "", "", "", "", "", "" },
                                new bool[] { false, true, true, true, true, true, true, true });
                    break;
                case "Minigames_Workout":
                    toSwitch = new MinigameSelection(MinigameSelection.MinigameType.ENERGY,
                                new String[] { "HeftyHeart", "", "", "", "", "", "", "" },
                                new bool[] { false, true, true, true, true, true, true, true });
                    break;
                case "Stress_Journal":
                    toSwitch = new StatusScreen();
                    break;
                case "HeftyHeart":
                    toSwitch = new HeftyHeart();
                    break;
                case "FoodForThought":
                    toSwitch = new FoodForThought();
                    break;
				case "VeinFlyer":
					toSwitch = new VeinFlyer();
					break;
				case "Exit":
                    ScreenStackManager.PopScene();
                    break;
            }

            if (toSwitch != null)
            {
                if (replace) ScreenStackManager.ReplaceScreen(toSwitch);
                else ScreenStackManager.AddScreen(toSwitch);
            }
        }
    }
}