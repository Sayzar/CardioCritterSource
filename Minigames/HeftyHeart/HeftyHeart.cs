using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CardioCritters.Code.GUI.Buttons;
using Microsoft.Xna.Framework.Input.Touch;
using CardioCritters.Code.GUI.Instructions;
using Microsoft.Xna.Framework.Input;
using CardioCritters.Code.GUI.Meter;
using System.Diagnostics;

namespace CardioCritters.Code.Minigames
{
    public class HeftyHeart : Minigame
    {
        public enum HEFTYHEARTSTATE
        {
            MENU,
            LIFT,
            STRETCH,
            LIFT2
        }

        private enum WEIGHTSTATE
        {
            BOTTOM,
            LIFTING,
            TOP
        }

        // Overall game variables
        public HEFTYHEARTSTATE state;
        public bool isStrength;
        private String prefix = "Minigame/HeftyHeart/";
        private Stopwatch timer;

        // Menu stuff
        private HeftyHeartButton endurance, strength;
        
        // Lifting stuff
        private Texture2D[] liftingFrames;
        public Entity heart, weight, reps, misses;
        public PopAndFade liftInstruction, stretchInstruction, good, miss;
        public int numReps, numMisses, totalReps, totalMisses;
        private Vector2 missesScorePos, repsScorePos, firstTouchLocation;
        public float weightDeltaYNeeded;
        private WEIGHTSTATE weightState;
        private bool legalRep, legalMiss;
        public float TimePerLiftInMS;


        // Stretching stuff
        private Texture2D[] stretchingFrames;
        private HeftyHeartMeter[] meters;
        private float[] distances;
        private float stretchDeltaNeeded;
        private Entity[] arrows;

        public HeftyHeart()
            : base(new String[] 
                    { "Minigame/HeftyHeart/instruction", 
                        "Minigame/HeftyHeart/instruction2", 
                        "Minigame/HeftyHeart/instruction3" 
                    }, 
                    "Minigame/HeftyHeart/background", 
                    "Minigame/HeftyHeart/gameover_bad", 
                    "Minigame/HeftyHeart/gameover_good", 
                    new Vector2(0,0))
        {
            // Start the game in the menu state
            state = HEFTYHEARTSTATE.MENU;

            timer = new Stopwatch();
            timer.Start();

            // Initialize the stuff for the MENU
            strength = new HeftyHeartButton(this, "Minigame/HeftyHeart/strength", "Minigame/HeftyHeart/strength2", true);
            strength.SetSizeRelativeToScreen(0.4f, 0.4f);
            strength.SetPosition(new Vector2(Utility.ScreenWidth/2 - strength.dimensions.Width, Utility.ScreenHeight/2 - strength.dimensions.Height/2));

            endurance = new HeftyHeartButton(this, "Minigame/HeftyHeart/endurance", "Minigame/HeftyHeart/endurance2", false);
            endurance.SetSizeRelativeToScreen(0.4f, 0.4f);
            endurance.SetPosition(new Vector2(Utility.ScreenWidth/2, Utility.ScreenHeight/2 - endurance.dimensions.Height/2));

            // Initialize the stuff for LIFTING part of the game
            liftingFrames = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                liftingFrames[i] = Utility.GetTexture(prefix + "Lift/" + (i + 1));

            heart = new Entity(prefix + "Stretch/idle");
            heart.renderIndex = 0.1f;
            heart.SetSizeRelativeToScreen(0.6f, 0.8f);
            heart.SetToCenterOfScreen();
            heart.Move(0, Utility.ScreenHeight / 8);

            weight = new Entity(prefix + "weights");
            weight.renderIndex = 0.0f;

            liftInstruction = new PopAndFade(false, prefix + "Lift/instruction", 1500f, 1f, 1f);

            reps = new Entity(prefix + "reps");
            reps.SetSizeRelativeToScreen(0.4f, 0.25f);
            reps.SetPosition(new Vector2(Utility.ScreenWidth / 2 - reps.dimensions.Width, 0));
            reps.renderIndex = 0.1f;

            misses = new Entity(prefix + "misses");
            misses.SetSizeRelativeToScreen(0.4f, 0.25f);
            misses.SetPosition(new Vector2(Utility.ScreenWidth / 2, 0));
            misses.renderIndex = 0.1f;

            numReps = numMisses = 0;
            repsScorePos = new Vector2(Utility.ScreenWidth / 2 - reps.dimensions.Width / 4, Utility.ScreenHeight / 15);
            missesScorePos = new Vector2(Utility.ScreenWidth / 2 + misses.dimensions.Width * 3 / 4, Utility.ScreenHeight/15);

            weightState = WEIGHTSTATE.BOTTOM;
            legalRep = false;
            legalMiss = true;
            

            // Initialize the stuff for STRETCHING part of the game
            stretchingFrames = new Texture2D[5];
            stretchingFrames[0] = Utility.GetTexture(prefix + "Stretch/idle");
            stretchingFrames[1] = Utility.GetTexture(prefix + "Stretch/right");
            stretchingFrames[2] = Utility.GetTexture(prefix + "Stretch/up");
            stretchingFrames[3] = Utility.GetTexture(prefix + "Stretch/left");
            stretchingFrames[4] = Utility.GetTexture(prefix + "Stretch/down");

            stretchInstruction = new PopAndFade(false, prefix + "Stretch/instruction", 1500f, 1f, 1f);

            distances = new float[4];
            stretchDeltaNeeded = Utility.ScreenWidth / 5;

            meters = new HeftyHeartMeter[4];
            arrows = new Entity[4];
            for (int i = 0; i < 4; i++)
            {
                meters[i] = new HeftyHeartMeter();
                meters[i].SetSizeRelativeToScreen(0.2f, 0.2f);
                meters[i].SetPosition(new Vector2(i * meters[i].dimensions.Width + (i+1) * Utility.ScreenWidth/20, Utility.ScreenHeight/10));

                arrows[i] = new Entity(prefix + "arrow" + (i + 1));
                arrows[i].SetSizeRelativeToScreen(0.05f, 0.075f);
                arrows[i].SetPosition(meters[i].GetCenter());
                arrows[i].Move(-arrows[i].dimensions.Width/2,-arrows[i].dimensions.Height*3/2);
                arrows[i].renderIndex = 0.3f;
            }

            good = new PopAndFade(false, prefix + "good", 1500f, 1f, 1f);
            miss = new PopAndFade(false, prefix + "miss", 1500f, 1f, 1f);
        }

        public override void DrawGame(SpriteBatch sb)
        {
            switch (state)
            {
                case HEFTYHEARTSTATE.MENU:
                    strength.Draw(sb);
                    endurance.Draw(sb);
                    break;
                case HEFTYHEARTSTATE.LIFT:
                    DrawLift(sb);
                    break;
                case HEFTYHEARTSTATE.STRETCH:
                    DrawStretch(sb);
                    break;
                case HEFTYHEARTSTATE.LIFT2:
                    DrawLift(sb);
                    break;
            }

            base.DrawGame(sb);
        }

        public override void UpdateGame(GameTime gameTime)
        {
            switch (state)
            {
                case HEFTYHEARTSTATE.MENU:
                    Vector2 pos = new Vector2(-1,-1);
                    foreach (TouchLocation loc in Utility.inputManager.GetTouches())
                        pos = loc.Position;
                    strength.HandleInput(pos);
                    endurance.HandleInput(pos);
                    break;
                case HEFTYHEARTSTATE.LIFT:
                    UpdateLift(gameTime);
                    break;
                case HEFTYHEARTSTATE.STRETCH:
                    UpdateStretch(gameTime);
                    break;
                case HEFTYHEARTSTATE.LIFT2:
                    UpdateLift(gameTime);
                    break;
            }
            base.UpdateGame(gameTime);
        }

        private void UpdateLift(GameTime gameTime)
        {
            

            // Handle the swipe actions to move the weight
#if WINDOWS
            // Some nice little hack to test this screen
            if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Left))
                MoveWeight(-3, gameTime);
            else if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Right))
                MoveWeight(3, gameTime);
#endif
            if (Utility.inputManager.GetGesture(GestureType.None))
                firstTouchLocation = new Vector2(-1, -1);

            // On a drag, we must move the world!
            if (Utility.inputManager.GetGesture(GestureType.VerticalDrag))
            {
                TouchLocation outTouch;
                Utility.inputManager.GetTouches()[0].TryGetPreviousLocation(out outTouch);
                if (firstTouchLocation.Equals(new Vector2(-1, -1)))
                    firstTouchLocation = Utility.inputManager.GetTouches()[0].Position;
                float deltaX = outTouch.Position.Y - Utility.inputManager.GetTouches()[0].Position.Y;
                MoveWeight(deltaX, gameTime);
            }

            good.Update(gameTime);
            miss.Update(gameTime);
            liftInstruction.Update(gameTime);
            heart.Update(gameTime);
            weight.Update(gameTime);
        }

        private void CheckForGood(float initial, float after)
        {
            if (initial < stretchDeltaNeeded && after >= stretchDeltaNeeded)
            {
                good.Reset();
                good.Start();
            }
        }

        private void UpdateStretch(GameTime gameTime)
        {
            // Check for game over
            bool temp = true;
            for (int i = 0; i < distances.Length; i++)
                temp &= distances[i] >= stretchDeltaNeeded;
            if (temp)
            {
                NextPhase();
                return;
            }

            good.Update(gameTime);
            miss.Update(gameTime);
            stretchInstruction.Update(gameTime);

            // On a drag, we must say that they are stretching
            if (Utility.inputManager.GetGesture(GestureType.VerticalDrag))
            {
                TouchLocation outTouch;
                Utility.inputManager.GetTouches()[0].TryGetPreviousLocation(out outTouch);
                if (firstTouchLocation.Equals(new Vector2(-1, -1)))
                    firstTouchLocation = Utility.inputManager.GetTouches()[0].Position;
                
                float deltaY = outTouch.Position.Y - Utility.inputManager.GetTouches()[0].Position.Y;
                //float deltaY = Utility.inputManager.GetTouches()[0].Position.Y - firstTouchLocation.Y;
                
                if (deltaY > 0)
                {
                    
                    CheckForGood(distances[3], distances[3] += 2);
                    heart.sprite = stretchingFrames[4];
                }
                else
                {
                    CheckForGood(distances[1], distances[1] += Math.Abs(2));
                    heart.sprite = stretchingFrames[2];
                }
            }
            else if (Utility.inputManager.GetGesture(GestureType.HorizontalDrag))
            {
                TouchLocation outTouch;
                Utility.inputManager.GetTouches()[0].TryGetPreviousLocation(out outTouch);
                if (firstTouchLocation.Equals(new Vector2(-1, -1)))
                    firstTouchLocation = Utility.inputManager.GetTouches()[0].Position;
                
                float deltaX = outTouch.Position.Y - Utility.inputManager.GetTouches()[0].Position.Y;
                //float deltaX = Utility.inputManager.GetTouches()[0].Position.X - firstTouchLocation.X;
                
                if (deltaX > 0)
                {
                    CheckForGood(distances[0], distances[0] += 2);
                    heart.sprite = stretchingFrames[1];
                }
                else
                {
                    CheckForGood(distances[2],distances[2] += Math.Abs(2));
                    heart.sprite = stretchingFrames[3];
                }
            }
            
            if (Utility.inputManager.GetGesture(GestureType.None)
                || Utility.inputManager.GetGesture(GestureType.DragComplete))
            {
                heart.sprite = stretchingFrames[0];
                firstTouchLocation = new Vector2(-1, -1);
            }

#if WINDOWS
            // Some nice little hack to test this screen
            if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Left))
            {
                CheckForGood(distances[2],distances[2] += Math.Abs(2));
                heart.sprite = stretchingFrames[3];
            }
            else if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Right))
            {
                CheckForGood(distances[0], distances[0] += 2);
                heart.sprite = stretchingFrames[1];
            }

            else if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Down))
            {
                CheckForGood(distances[3], distances[3] += 2);
                heart.sprite = stretchingFrames[4];
            }
            else if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Up))
            {
                CheckForGood(distances[1], distances[1] += Math.Abs(2));
                heart.sprite = stretchingFrames[2];
            }
            else
            {
                heart.sprite = stretchingFrames[0];
            }
#endif

            for (int i = 0; i < 4; i++)
                meters[i].Update(gameTime, MathHelper.Clamp(distances[i] / stretchDeltaNeeded, 0f, 1f));
        }

        private void DrawLift(SpriteBatch sb)
        {
            heart.Draw(sb);
            weight.Draw(sb);
            liftInstruction.Draw(sb);
            reps.Draw(sb);
            misses.Draw(sb);
            good.Draw(sb);
            miss.Draw(sb);


            sb.DrawString(Game1.font, numReps.ToString(), repsScorePos, Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.0f);
            sb.DrawString(Game1.font, numMisses.ToString(), missesScorePos, Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.0f);
        }

        private void DrawStretch(SpriteBatch sb)
        {
            String temp = heart.sprite.Name;


            good.Draw(sb);
            miss.Draw(sb);
            heart.Draw(sb);
            stretchInstruction.Draw(sb);

            for (int i = 0; i < 4; i++)
            {
                meters[i].Draw(sb);
            }

            

        }

        private void MoveWeight(float deltaY, GameTime gameTime)
        {

            // Move it, dont worry about boundary because its already taken care of
            weight.Move(0, -deltaY);

            // Check to see if the deltaY was too large, therefore a miss
            float maxDeltaYPerMS = weightDeltaYNeeded / TimePerLiftInMS;
            if (legalMiss && Math.Abs(deltaY) > Math.Abs(maxDeltaYPerMS * gameTime.ElapsedGameTime.Milliseconds))
            {
                // Reset and play the miss thing
                miss.Reset();
                miss.Start();

                legalMiss = false;
                numMisses++;
                totalMisses++;

                // Put the weight at the bottom, lulz
                weight.SetPosition(new Vector2(weight.minX, weight.maxY));
                legalRep = false;
            }
           

            // After we move the weight, we want to adjust the heart's texture if needed
            AdjustHeart();

            // If we hit the bottom, then we are in the BOTTOM state
            if (weight.hitBoundary && weight.GetPosition().Y + weight.dimensions.Height >= weight.maxY)
            {
                // If we hit the top, and we were lifting, and its a legal rep, that means we did a rep
                if (legalRep && weightState == WEIGHTSTATE.LIFTING)
                {
                    // We do this to make sure they also hit BOTTOM again before another rep
                    legalRep = false;
                    legalMiss = true;

                    numReps++;
                    totalReps++;

                    // Win condition 
                    if (isStrength && numReps >= 10)
                        NextPhase();
                    else if (numReps >= 15)
                        NextPhase();
                    else
                    {
                        good.Reset();
                        good.Start();
                    }
                }

                weightState = WEIGHTSTATE.BOTTOM;
            }
            // If we hit the top, then we are in the TOP state
            else if (weight.hitBoundary && weight.GetPosition().Y <= weight.minY)
            {

                // It becomes a legal rep when we were lifting and then hit the top 
                // (I know this is useless, just makes the logic more apparent :3)
                if (weightState == WEIGHTSTATE.LIFTING)
                    legalRep = true;
                    
                weightState = WEIGHTSTATE.TOP;
            }
            // Otherwise we will be lifting it!
            else
            {
                weightState = WEIGHTSTATE.LIFTING;
            }
        }

        // This will only be called on the LIFT/STRETCH/LIFT2 cases
        private void NextPhase()
        {
            // Lift -> Stretch
            if (state == HEFTYHEARTSTATE.LIFT)
            {
                state = HEFTYHEARTSTATE.STRETCH;
                stretchInstruction.Start();
                heart.sprite = stretchingFrames[0];
                good.Reset();
                miss.Reset();
            }
            // Stretch -> Lift2
            else if (state == HEFTYHEARTSTATE.STRETCH)
            {
                heart.sprite = liftingFrames[0];
                // Put the weight at the bottom, lulz
                weight.SetPosition(new Vector2(weight.minX, weight.maxY));
                state = HEFTYHEARTSTATE.LIFT2;
                miss.Reset();
                good.Reset();
                liftInstruction.Reset();
                liftInstruction.Start();
                numReps = 0;
                numMisses = 0;
            }
            else if (state == HEFTYHEARTSTATE.LIFT2)
            {
                score = 40000 - timer.ElapsedMilliseconds;
                win = score >= 0; // 40 seconds to finish
                score -= 2000 * totalMisses;
                
                scorePosition = new Vector2(Utility.ScreenWidth/3 + Utility.ScreenWidth/20, Utility.ScreenHeight/2 - Game1.font.MeasureString(score.ToString()).Y);
                SetState(STATE.GAMEOVER);
            }
        }

        private void AdjustHeart()
        {
            //Set the heart's sprite to the correct texture according to how far the player has lifted the weight
            int p1 = (weight.maxY - weight.dimensions.Height);
            int p2 = p1 - (int) weightDeltaYNeeded;
            int p3 = (int) weight.GetPosition().Y;
            int ratio = liftingFrames.Length - 1;

            // Small hack for a bug that was happening as it approached 0
            if(p3 - p2 >= 5)
                ratio = ((p1 - p2) - (p3 - p2)) / (p3 - p2);


            ratio = (int) MathHelper.Clamp(ratio, 0, liftingFrames.Length - 1);
            heart.sprite = liftingFrames[ratio];
        }
    }
}