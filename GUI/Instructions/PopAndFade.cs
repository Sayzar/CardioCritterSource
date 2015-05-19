using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace CardioCritters.Code.GUI.Instructions
{
    public class PopAndFade : Entity
    {
        private Stopwatch timer;
        private float time;
        private float finalWidth, finalHeight;
        private bool fadeIn;

        public PopAndFade(bool fadeIn, String spriteFilepath, float timeInMS, float finalWidth, float finalHeight)
            :base(spriteFilepath)
        {
            this.SetScale(Vector2.Zero);
            this.time = timeInMS;
            this.timer = new Stopwatch();
            this.finalHeight = finalHeight;
            this.finalWidth = finalWidth;
            this.fadeIn = fadeIn;
        }

        public void Start()
        {
            timer.Start();
        }

        public override void Update(GameTime gameTime)
        {
            if (!timer.IsRunning) return;

            float ratioComplete = timer.ElapsedMilliseconds / time;
            this.opacity = fadeIn ? ratioComplete : 1 - ratioComplete;
            if (fadeIn && this.opacity >= 1f)
            {
                this.opacity = 1f;
                timer.Stop();
            }
            else if (!fadeIn && this.opacity <= 0f)
            {
                this.opacity = 0f;
                timer.Stop();
            }

            this.SetSizeRelativeToScreen(ratioComplete * finalWidth, ratioComplete * finalHeight);
            SetToCenterOfScreen();

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if(timer.IsRunning)
                base.Draw(sb);
        }

        public void Reset()
        {
            timer = new Stopwatch();
        }
    }
}
