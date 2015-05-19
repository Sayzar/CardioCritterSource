using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;
using XnaUtility;
using CardioCritters;

namespace CardioCritters.Code.Heart
{
    public class Pet : AnimatedEntity
    {
        private Vector2 direction;
        private Random r;
        private Stopwatch timer, timer2;
        private Dictionary<string, SSAnimatedEntity> accessoriesEquipped;

        public Pet()
            : base("Heart/Tap/1")
        {
            // Load all the animations
            AddAnimation("Tap", "Heart/Tap/", 10, 1f, false);
            AddAnimation("Tickle", "Heart/Tickle/", 10, 2f, false);
            AddAnimation("Idle_Happy", "Heart/Idle_Happy/", 9, 2f, false);
            AddAnimation("Idle_Ok", "Heart/Idle_Ok/", 10, 2f, false);
            AddAnimation("Idle_Sad", "Heart/Idle_Sad/", 6, 2f, false);

            PlayAnimation("Idle_Ok");

            IdleAnimationName = "Idle_Ok";

            // accessory stuff
            accessoriesEquipped = new Dictionary<string, SSAnimatedEntity>();
            ///////////////////////////////
            // JUST TESTING HAT FOR NOW
            // TODO: make a method that automates this

            String[] hatAnimNames = { "Tap", "Tickle", "Idle_Happy", "Idle_Ok", "Idle_Sad", "Idle_Sick" };

            SSAnimatedEntity hatAnim = new SSAnimatedEntity("AccessoriesSS", "Hat", hatAnimNames);
            hatAnim.SetAnimationInfo("Tap", 1f, false);
            hatAnim.SetAnimationInfo("Tickle", 2f, false);
            hatAnim.SetAnimationInfo("Idle_Happy", 2f, false);
            hatAnim.SetAnimationInfo("Idle_Ok", 2f, false);
            hatAnim.SetAnimationInfo("Idle_Sad", 2f, false);

            //             string filepath = "Accessories/Hat/";
            //             AnimatedEntity accAnim = new AnimatedEntity(filepath + "Tap/1");
            //             accAnim.AddAnimation("Tap", filepath + "Tap/", 10, 1f, false);
            //             accAnim.AddAnimation("Tickle", filepath + "Tickle/", 10, 2f, false);
            //             accAnim.AddAnimation("Idle_Happy", filepath + "Idle_Happy/", 9, 2f, false);
            //             accAnim.AddAnimation("Idle_Ok", filepath + "Idle_Ok/", 10, 2f, false);
            //             accAnim.AddAnimation("Idle_Sad", filepath + "Idle_Sad/", 6, 2f, false);
            //             accAnim.renderIndex = 0f;

            accessoriesEquipped.Add("Hat", hatAnim);

            //accAnim.AddAnimation("Tap", filepath + "Tap/", 10, 1f, false);
            //accAnim.AddAnimation("Tickle", filepath + "Tickle/", 10, 2f, false);
            //accAnim.AddAnimation("Idle_Happy", filepath + "Idle_Happy/", 9, 2f, false);
            //accAnim.AddAnimation("Idle_Ok", filepath + "Idle_Ok/", 10, 2f, false);
            //accAnim.AddAnimation("Idle_Sad", filepath + "Idle_Sad/", 6, 2f, false);
            //accAnim.renderIndex = 0f;
            //
            //accessoriesEquipped.Add("Hat", accAnim);
            //
            //accAnim.AddAnimation("Tap", filepath + "Tap/", 10, 1f, false);
            //accAnim.AddAnimation("Tickle", filepath + "Tickle/", 10, 2f, false);
            //accAnim.AddAnimation("Idle_Happy", filepath + "Idle_Happy/", 9, 2f, false);
            //accAnim.AddAnimation("Idle_Ok", filepath + "Idle_Ok/", 10, 2f, false);
            //accAnim.AddAnimation("Idle_Sad", filepath + "Idle_Sad/", 6, 2f, false);
            //accAnim.renderIndex = 0f;
            //
            //accessoriesEquipped.Add("Hat", accAnim);
            //
            //accAnim.AddAnimation("Tap", filepath + "Tap/", 10, 1f, false);
            //accAnim.AddAnimation("Tickle", filepath + "Tickle/", 10, 2f, false);
            //accAnim.AddAnimation("Idle_Happy", filepath + "Idle_Happy/", 9, 2f, false);
            //accAnim.AddAnimation("Idle_Ok", filepath + "Idle_Ok/", 10, 2f, false);
            //accAnim.AddAnimation("Idle_Sad", filepath + "Idle_Sad/", 6, 2f, false);
            //accAnim.renderIndex = 0f;
            //
            //accessoriesEquipped.Add("Hat", accAnim);
            //////////////////////////////

            // Make sure its bounded
            SetBounds(0, Utility.ScreenWidth * 2, 0, Utility.ScreenHeight, true);

            // Make sure it is always drawn first
            renderIndex = 0.1f;

            // AI Stuff
            direction = new Vector2(0, 1);
            r = new Random();
            timer = new Stopwatch();
            timer2 = new Stopwatch();
            timer.Start();
            timer2.Start();
        }

        public override void Update(GameTime gameTime)
        {
            // Every 2 seconds, the AI will want to decide to switch directions or not
            if (timer.ElapsedMilliseconds >= 2000)
            {
                direction.X = (r.Next(3) - 1) * 3;
                spriteEffect = direction.X >= 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                timer.Restart();
            }

            // Every half second, we will want to change the up/down
            if (timer2.ElapsedMilliseconds >= 500)
            {
                direction.Y = direction.Y * -1;
                timer2.Restart();
            }

            // Move the heart and force the bounds
            Move(direction.X, direction.Y);

            //ForceBounds(); //Forcebounds is in base.Update...

            // Always update the base
            base.Update(gameTime);

            Vector2 curPos = this.GetPosition();
            Vector2 curScale = this.GetCurrentScale();

            foreach (SSAnimatedEntity en in accessoriesEquipped.Values)
            {
                // we should only be doing this only when we just modify this heart entity,
                // but for code's simplicity, just set it like this for now
                en.Scale = curScale;
                en.Position = curPos;
                en.SpriteEffect = spriteEffect;
                en.Rotation = rotation;
                en.CurAnimName = CurrentAnimationName;
                en.FrameOverride = currentAnimation.renderIndex;
                en.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            int curIndex = 0;
            curIndex = this.currentAnimation.renderIndex;

            String animName = this.CurrentAnimationName;

            foreach (SSAnimatedEntity en in accessoriesEquipped.Values)
            {
                en.Draw(sb);
            }
        }

        public void HandleInput(GestureType gesture)
        {
            if (gesture == GestureType.Tap)
                PlayAnimation("Tap");
            if (gesture == GestureType.Pinch || gesture == GestureType.VerticalDrag || gesture == GestureType.HorizontalDrag)
                PlayAnimation("Tickle");
        }
    }
}