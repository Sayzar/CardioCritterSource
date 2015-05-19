using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardioCritters.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CardioCritters.Code.Minigames
{
    public class FallingGrub : Screen
    {
        private List<Entity> good, bad, badCaught;
        private Entity heart, fillbar;
        private TimeSpan timer = new TimeSpan();
        private Random generator;
        private int score;
        private int difficulty;

        public FallingGrub()
            : base("Minigame/Fallinggrub/darkbgeat")
        {
#if ANDROID
            Utility.inputManager.StartAccel();
#endif

            // Set the game speed to be the first speed in Config
            difficulty = 0;

            Utility.AddNewTexture("Heart/NomNom/1");
            Utility.AddNewTexture("Minigame/Fallinggrub/fillbar");
            
            // The heart that the player controls
            // Add the Eat and Idle animation frames
            heart = new Entity("Heart/NomNom/1");
            heart.SetSizeRelativeToScreen(0.1f, 0.2f);
            heart.SetPosition(new Vector2(Utility.ScreenWidth/2 - heart.dimensions.Width/2, 
                Utility.ScreenHeight - heart.dimensions.Height));
            //heart.CreateAnimation("Art/Heart/NomNom", "Eat", 2f, 13, false);
            //heart.CreateAnimation("Art/Heart/NomNom", "Idle", 2f, 1, true);
            heart.SetBoundsToScreen();

            // The UI on the right side of the screen
            fillbar = new Entity("Minigame/Fallinggrub/fillbar");
            fillbar.SetSizeRelativeToScreen(0.1f, 0.5f);
            fillbar.SetPosition(new Vector2(Utility.ScreenWidth - fillbar.dimensions.Width, 
                0));

            // Initialize the arrays
            good = new List<Entity>();
            bad = new List<Entity>();
            badCaught = new List<Entity>();
            
            score = 0;

            String[] textToAdd = {"Minigame/Fallinggrub/baditem_01", "Minigame/Fallinggrub/baditem_02",
                                   "Minigame/Fallinggrub/gooditem_01", "Minigame/Fallinggrub/gooditem_02",
                                   "Minigame/Fallinggrub/gooditem_03"
                               };
            foreach (String s in textToAdd)
                Utility.AddNewTexture(s);

            timer = TimeSpan.Zero;
            generator = new Random(DateTime.Now.Millisecond);
        }

        public override void Update(GameTime gameTime)
        {
            // timer
            timer -= gameTime.ElapsedGameTime;

            // Update the heart
            heart.Update(gameTime);

            MovePlayer(gameTime);

            // When the sprite is done eating, then reset the animation to the idle frame
            //if (heart.sprite.ActiveAction == heart.GetAnimation("Eat") && heart.sprite.ActiveAction.isDone())
            //    heart.SetAnimation("Idle");

            // Game over if the player catches 5 of the bad things
            if (badCaught.Count >= 5)
            {
                //SoundEngine.sharedEngine().stopSound();
                //SoundEngine.sharedEngine().realesAllSounds();
                //CCDirector.sharedDirector().replaceScene(GameOver.scene(new CCSprite("Art/Background/foodGameOver.png"), new CGPoint(2 * Main.ScreenSize.width / 3, Main.ScreenSize.height / 6), score));
            }

            // For every entity that is BAD
            for (int i = bad.Count -1; i >= 0 ; i--)
            {
                Entity e = bad[i];
                e.Move(0, Configuration.FF_SPEEDS[difficulty]);

                // If the player intersects with the food item while it is at least half above them
                if (e.IntersectsEntity(heart) && e.GetPosition().Y + e.dimensions.Height / 2 >= heart.GetPosition().Y)
                {
                    // Play the eat animation
                    //heart.SetAnimation("Eat");

                    // Resize and add to the fillbar's array to display. Remove it from the items on the field
                    e.SetSizeRelativeToScreen(0.065f, 0.095f);
                    badCaught.Add(e);
                    bad.RemoveAt(i);

                    // Also, play the blah sound
                    //SoundEngine.sharedEngine().playEffect(CCDirector.sharedDirector().getActivity(), R.raw.blah);
                }

                // Always check to see if the food item should dissapear
                else if (e.GetPosition().Y >= Utility.ScreenHeight)
                {
                    bad.RemoveAt(i);
                }
            }

            // For every entity that is GOOD
            for (int i = good.Count -1 ; i >= 0; i--)
            {

                Entity e = good[i];
                e.Move(0, Configuration.FF_SPEEDS[difficulty]);

                // If the player intersects with the food item while it is at least half above them
                if (e.IntersectsEntity(heart) && e.GetPosition().Y + e.dimensions.Height / 2 >= heart.GetPosition().Y)
                {

                    // Play the eat animation
                    //heart.SetAnimation("Eat");

                    // Update the score to add however many points
                    score += Configuration.FF_POINTS_PER_ITEM;

                    // Remove it from the field and play the bite sound
                    good.RemoveAt(i);

                    //SoundEngine.sharedEngine().playEffect(CCDirector.sharedDirector().getActivity(), R.raw.bite);
                }
                else if (e.GetPosition().Y >= Utility.ScreenHeight)
                {
                    good.RemoveAt(i);
                }
            }

            // Do all the spawning logic (go to Configuration.java to mess with these values!) based 
            // upon the current difficulty of the game
            if (timer <= TimeSpan.Zero)
            {
                timer = new TimeSpan(0,0,0,0,Configuration.FF_TO_SPAWN_TIME_DIFFERENCE_IN_MS[difficulty]);
                if (generator.NextDouble() <= Configuration.FF_BAD_SPAWN_RATIO[difficulty]) Spawn(false);
                else Spawn(true);
            }

            // Check to see if we should move up a difficulty
            if (score >= Configuration.FF_DIFFICULTY_BY_POINTS[difficulty]) difficulty++;
            if (difficulty >= Configuration.FF_SPEEDS.Length - 1) difficulty = Configuration.FF_SPEEDS.Length - 1;

            // Set the positions of all the bad things we have accumulated
            // I know this only has to be done once, but there's not much going on so we can leave this here for now,
            // but if we need optimizations, this is a good place to start
            for (int i = 0; i < badCaught.Count; i++)
            {
                Entity e = badCaught[i];
                e.SetPosition(new Vector2(fillbar.dimensions.X + e.dimensions.Width / 4,
                    fillbar.dimensions.Y + e.dimensions.Height * i));
            }
        }

        private void MovePlayer(GameTime gameTime)
        {
            Vector3 accel = Utility.inputManager.GetAccelReading();

            float accelY = accel.Y;

            // Move the heart if the accelerometer changes enough and by that fraction
            if (accelY >= Configuration.FF_ACCELEROMETER_CHANGE_NEEDED_TO_MOVE)
                heart.Move(Configuration.FF_MOVEMENT_SPEED, 0);
            else if (accelY <= -Configuration.FF_ACCELEROMETER_CHANGE_NEEDED_TO_MOVE)
                heart.Move(-Configuration.FF_MOVEMENT_SPEED, 0);
        }

        // Spawns an item at a random location, bad or good is given by the parameter. Also adds the item
        // to the approprite list
        private void Spawn(bool goodItem)
        {
            Entity item;
            if (goodItem)
            {
                int r = generator.Next(1, 4);
                item = new Entity("Minigame/Fallinggrub/gooditem_0" + r );
                item.SetPosition(new Vector2(generator.Next((int)Utility.ScreenWidth - (int)item.dimensions.Width), 
                    0));
                item.SetSizeRelativeToScreen(Configuration.FF_FALLING_ITEM_WIDTH, Configuration.FF_FALLING_ITEM_HEIGHT);
                good.Add(item);
            }
            else
            {
                int r = generator.Next(1, 3);
                item = new Entity("Minigame/Fallinggrub/baditem_0" + r );
                item.SetPosition(new Vector2(generator.Next((int)Utility.ScreenWidth - (int)item.dimensions.Width), 
                    0));
                item.SetSizeRelativeToScreen(Configuration.FF_FALLING_ITEM_WIDTH, Configuration.FF_FALLING_ITEM_HEIGHT);
                bad.Add(item);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            base.Draw(sb);
            
            SpriteFont font = Game1.font;
            String textScore = "Score: " + score;
            Vector2 scoreSize = font.MeasureString(textScore);

            heart.Draw(sb);
            foreach (Entity s in good)
                s.Draw(sb);
            foreach (Entity s in bad)
                s.Draw(sb);

            // UI last drawn to be on top
            // concern : if item is dropping on the UI, UI might block it
            fillbar.Draw(sb);

            foreach (Entity s in badCaught)
                s.Draw(sb);

            sb.DrawString(font, textScore,
                new Vector2(Utility.ScreenWidth / 2 - scoreSize.X / 2, 0),
                Color.White);
             
        }
    }
}