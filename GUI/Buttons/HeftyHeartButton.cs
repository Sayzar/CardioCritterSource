using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardioCritters.Code.Minigames;

namespace CardioCritters.Code.GUI.Buttons
{
    public class HeftyHeartButton : Button
    {
        private HeftyHeart parent;
        private bool isStrength;
        
        public HeftyHeartButton(HeftyHeart parent, String nonPressed, String pressed, bool isStrength)
            : base(nonPressed, pressed)
        {
            this.parent = parent;
            this.isStrength = isStrength;
        }

        public override void OnPress()
        {
            this.parent.isStrength = this.isStrength;
            this.parent.state = HeftyHeart.HEFTYHEARTSTATE.LIFT;

            if (this.parent.isStrength)
            {
                this.parent.weight.SetSizeRelativeToScreen(0.3f, 0.4f);
                this.parent.TimePerLiftInMS = 100f;
            }
            else
            {
                this.parent.weight.SetSizeRelativeToScreen(0.25f, 0.3f);
                this.parent.TimePerLiftInMS = 70f;
            }

            Entity weight = this.parent.weight;
            this.parent.weightDeltaYNeeded = isStrength ? Utility.ScreenHeight / 6 : Utility.ScreenHeight / 8;
            weight.SetToCenterOfScreen();
            weight.Move(0, this.parent.heart.dimensions.Height / 4);

            weight.SetBounds(weight.dimensions.X, weight.dimensions.X + weight.dimensions.Width,
                weight.dimensions.Y - (int)this.parent.weightDeltaYNeeded, weight.dimensions.Y + weight.dimensions.Height, true);

            this.parent.liftInstruction.Start();
            
            
        }
    }
}