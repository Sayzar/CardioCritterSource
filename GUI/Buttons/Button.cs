using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;

namespace CardioCritters.Code.GUI
{
    public abstract class Button : Entity
    {
        private Texture2D nonPressed, pressed;
        private bool isPressed;
        private Stopwatch timer;

        public Button(String nonPressed, String pressed) : base(nonPressed)
        {
            this.nonPressed = Utility.GetTexture(nonPressed);
            this.pressed = Utility.GetTexture(pressed);
            isPressed = false;
            renderIndex = 0.9f;
            timer = new Stopwatch();
        }

        public void HandleInput(Vector2 touchLoc)
        {
            if (isPressed && (Utility.inputManager.GetGesture(GestureType.None)
                            || Utility.inputManager.GetTouches().ToArray().Length == 0
                            || !(this.Intersects(Utility.inputManager.GetTouches()[0].Position)))) OnRelease();
            if (this.Intersects(touchLoc)) PrePress();
            if (this.Intersects(touchLoc) && Utility.inputManager.GetGesture(GestureType.Tap)) OnPress();
        }

        private void PrePress()
        {
            isPressed = true;
            timer.Start();
        }

        public abstract void OnPress();

        public virtual void OnRelease()
        {
            isPressed = false;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isPressed) sprite = pressed;
            else sprite = nonPressed;

            base.Draw(sb);
        }
    }
}