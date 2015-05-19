using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardioCritters.Screens;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using CardioCritters.Code.Minigames;
using Microsoft.Xna.Framework;

namespace CardioCritters
{
    class Splash : Screen
    {
        private Stopwatch timer;
        private Entity splashLogo;
        private SplashState state;
        private int opactiyControl;

        private enum SplashState
        {
            FADING_IN,
            SOLID,
            FADING_OUT
        }

        public bool IsLoadingThreadDone
        {
            private get { return isLoadingThreadDone; }
            set { isLoadingThreadDone = value; }
        }
        private volatile bool isLoadingThreadDone = false;

        public Splash()
            : base(null)
        {
            timer = new Stopwatch();
            state = SplashState.FADING_IN;
            splashLogo = new Entity("SplashScreen/splash");
            splashLogo.SetSizeRelativeToScreen(0.3f, 0.5f);
            splashLogo.SetToCenterOfScreen();
            opactiyControl = 0;
        }

        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case SplashState.FADING_IN:
                    opactiyControl += Configuration.OPACITY_CHANGE;
                    splashLogo.opacity = opactiyControl / 255f;
                    //splashLogo.SetRotationDegrees(splashLogo.GetRotationDegrees() + 5);
                    if (splashLogo.opacity >= 1f)
                    {
                        opactiyControl = 255;
                        state = SplashState.SOLID;
                        timer.Start();
                    }
                    //if (splashLogo.GetRotationDegrees() >= 720)
                    //{
                    //    splashLogo.SetRotationDegrees(720);
                    //    state = SplashState.SOLID;
                    //    timer.Start();
                    //}
                    break;
                case SplashState.SOLID:
                    if (timer.ElapsedMilliseconds >= Configuration.SOLID_TIME)
                    {
                        if (IsLoadingThreadDone)
                            state = SplashState.FADING_OUT;
                        else
                        {
                            //Idle animation
                            //double offset = Math.Sin((double)(Configuration.SOLID_TIME - timer.ElapsedMilliseconds)/100);
                            //splashLogo.SetRotationDegrees(splashLogo.GetRotationDegrees() - (float)offset);

                            //TODO: Also draw flashing "Loading" text?
                        }
                    }
                    break;
                case SplashState.FADING_OUT:
                    opactiyControl -= Configuration.OPACITY_CHANGE;
                    splashLogo.opacity = opactiyControl / 255f;
                    //splashLogo.SetRotationDegrees(splashLogo.GetRotationDegrees() + 5);
                    if (splashLogo.opacity <= 0f)
                    {
                        ScreenStackManager.ReplaceScreen(new MainMenu());
                    }
                    break;
            }

        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            splashLogo.Draw(sb);
        }
    }
}