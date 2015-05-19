using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardioCritters.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CardioCritters.Code.Minigames
{
    public class Rollout : Screen
    {
        // Array of enemies currently in the world
        private List<Entity> enemies;

        // For some collision code
        private bool LEFT, UP;

        // For the score
        //private CCLabel scoreLabel;
        private int collected;

        // Timers for spawning
        private TimeSpan gameTimer, spawnTimer;
        private int amountToSpawn;

        // The entities in this game
        private Entity background, heart, hunter;
        private Random r = new Random();

        // This is the indicator for what size the world is in
        private int currentSize;

        public Rollout()
            : base("Background/rolloutGameBackground")
        {
            //this.setIsTouchEnabled(true);
            //this.setIsAccelerometerEnabled(true);
#if ANDROID
            Utility.inputManager.StartAccel();
#endif

            // Play the rollout song
            //SoundEngine.sharedEngine().playSound(CCDirector.sharedDirector().getActivity(), R.raw.rollout, true);

            //Initialize everything
            LEFT = UP = false;

            enemies = new List<Entity>();

            //background = new Entity(("Background/rolloutGameBackground"));
            //background.SetSizeRelativeToScreen(1f, 1f);

            //scoreLabel = CCLabel.makeLabel("Score: 0", "Font/Fibel Nord.ttf", 72);
            //scoreLabel.setAnchorPoint(0, 0);
            //scoreLabel.setPosition(new CGPoint(9 * Main.ScreenSize.width / 20, 17 * Main.ScreenSize.height / 20));
            //addChild(scoreLabel);

            gameTimer = new TimeSpan();
            spawnTimer = new TimeSpan();

            heart = new Entity(("Heart/Idle/1"));
            heart.SetSizeRelativeToScreen(Configuration.CC_HEART_SIZES_X[currentSize], Configuration.CC_HEART_SIZES_Y[currentSize]);
            heart.SetPosition(new Vector2(Utility.ScreenWidth / 2 - heart.dimensions.Width / 2, Utility.ScreenHeight / 2 - heart.dimensions.Height / 2));

            hunter = new Entity(("Minigame/Rollout/zorro"));
            hunter.SetSizeRelativeToScreen(Configuration.CC_HEART_SIZES_X[currentSize], Configuration.CC_HEART_SIZES_Y[currentSize]);
            hunter.SetBoundsToScreen();

            currentSize = 0;

            //Inject the update method
            //this.schedule("update");
        }


        public override void Update(GameTime gameTime)
        {
            //if (!gameTimer.running) gameTimer.start();
            //if (!spawnTimer.running) spawnTimer.start();
            
            gameTimer += gameTime.ElapsedGameTime;
            spawnTimer += gameTime.ElapsedGameTime;

            MovePlayer(gameTime);

            // Game only lasts 1 minute (change in config now)
            if (gameTimer.TotalSeconds >= Configuration.CC_GAME_TIME)
            {
                // GAME OVER
                //CCDirector.sharedDirector().replaceScene(GameOver.scene(new CCSprite("Art/Background/rolloverGameOver.png"), new CGPoint(2 * Main.ScreenSize.width / 3, Main.ScreenSize.height / 6), collected));
                //SoundEngine.sharedEngine().stopSound();
                //SoundEngine.sharedEngine().realesAllSounds();

            }
            else
                // The amount of enemies to spawn should be in correlation to time
                // Every 10 seconds the game will spawn a tier higher
                // 6 tiers of spawn amounts
                amountToSpawn = Configuration.CC_TO_SPAWN[(int)MathHelper.Clamp((int)gameTimer.TotalSeconds / 10, 0, 5)];


            // Spawn every so often (set in config)
            if (spawnTimer.TotalSeconds >= Configuration.CC_SPAWN_INTERVAL)
            {
                spawnTimer = TimeSpan.Zero;
                Spawn();
            }

            // Always set the scale to the currentsize
            for (int i = 0; i < Configuration.CC_KILLS_NEEDED_TO_GROW.Length; i++)
            {
                if (collected >= Configuration.CC_KILLS_NEEDED_TO_GROW[i])
                    currentSize = i;
            }
            SetScaleOfGame(currentSize);

            // Enforce the idea that the hunter cannot be on the heart
            if (hunter.IntersectsEntity(heart))
            {
                int x = 0, y = 0;
                // They collided from the top/bottom
                if (Math.Pow(heart.GetPosition().X - hunter.GetPosition().X, 2) >= Math.Pow(heart.GetPosition().Y - hunter.GetPosition().Y, 2))
                {
                    x = (int)hunter.GetPosition().X;
                    if (UP) y = (int)heart.GetPosition().Y - hunter.dimensions.Height;
                    else y = (int)heart.GetPosition().Y + heart.dimensions.Height;
                }
                // They collided from the left/right
                else
                {
                    y = (int)hunter.GetPosition().Y;
                    if (LEFT) x = (int)heart.GetPosition().X + heart.dimensions.Width;
                    else x = (int)heart.GetPosition().X - hunter.dimensions.Width;

                }

                hunter.SetPosition(x, y);
            }


            // For every enemy that we still care about
            for (int i = enemies.Count - 1; i >= 0; i--)
            {

                // Get the enemy
                Entity e = enemies[i];

                // Move it towards the middle position of the heart (fractions of the width of the screen)
                e.MoveTo(heart.GetCenter(), 5);//(int)(Main.ScreenSize.width * Configuration.CC_ENEMY_SPEEDS[currentSize]));

                // If the heart gets hit by the enemy
                if (e.IntersectsEntity(heart))
                {
                    // We reduce the score by 1 and update it
                    collected--;
                    //updateScore();

                    // We hide/cleanup the enemy and remove it from our list
                    enemies.RemoveAt(i);
                    //e.sprite.setVisible(false);

                    // Lastly, play the sound effect
                    //SoundEngine.sharedEngine().playEffect(CCDirector.sharedDirector().getActivity(), R.raw.scream);

                    // We do not want a situation that both occur on the same frame
                    continue;
                }

                // On the other hand, if the hunter gets to it first
                if (e.IntersectsEntity(hunter))
                {

                    // Update the score to 1 more
                    collected++;
                    //updateScore();

                    //  Hide/Cleanup the enemy
                    enemies.RemoveAt(i);
                    //e.sprite.setVisible(false);

                    // Play the sound effect that is needed
                    //SoundEngine.sharedEngine().playEffect(CCDirector.sharedDirector().getActivity(), R.raw.grunt);
                }
            }

            // Update the warrior (mainly for the bound checking!)
            hunter.Update(gameTime);
            
        }

        private void MovePlayer(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.Seconds;
            Vector3 accel = Utility.inputManager.GetAccelReading();
            float accelY = accel.Y;
            float accelX = accel.X;

            LEFT = (accelY >= 0) ? false : true;
            UP = (accelX >= 0) ? false : true;

            if (Math.Abs(accelY) >= Configuration.CC_TILT_NEEDED)
                hunter.Move(((accelY >= 0) ? 1 : -1) * Utility.ScreenHeight * Configuration.CC_HUNTER_SPEEDS[currentSize], 0);

            if (Math.Abs(accelX) >= Configuration.CC_TILT_NEEDED)
                hunter.Move(0, ((accelX >= 0) ? 1 : -1) * Utility.ScreenWidth * Configuration.CC_HUNTER_SPEEDS[currentSize]);
        }

        // Sets the world's scale!
        private void SetScaleOfGame(int currentSize2) 
        {
		    // First, we must set the heart in the middle
		    heart.SetSizeRelativeToScreen(Configuration.CC_HEART_SIZES_X[currentSize], Configuration.CC_HEART_SIZES_Y[currentSize]);
		    heart.SetToCenterOfScreen();
		
		    // Now we must change all the enemies that are currently out there
		    foreach(Entity e in enemies)
			    e.SetSizeRelativeToScreen(Configuration.CC_ENEMY_SIZES_X[currentSize], Configuration.CC_ENEMY_SIZES_Y[currentSize]);
	    }

        // Spawns the appropriate amount of enemies
        private void Spawn()
        {

            // For as many enemies that we need to spawn
            for (int i = 0; i < amountToSpawn; i++)
            {
                // Randomly somewhere in the bounds of the screen
                int x, y;
                if (r.Next(2) == 1)
                {
                    x = r.Next(Utility.ScreenWidth);
                    y = r.Next(2) == 1 ? -(int)(Utility.ScreenHeight * Configuration.CC_ENEMY_SIZES_Y[currentSize])
                        : (int)Utility.ScreenHeight - (int)(Utility.ScreenHeight * Configuration.CC_ENEMY_SIZES_Y[currentSize]);
                }
                else
                {
                    x = r.Next(2) == 1 ? -(int)(Utility.ScreenWidth * Configuration.CC_ENEMY_SIZES_X[currentSize]) 
                        : (int)Utility.ScreenWidth - (int)(Utility.ScreenWidth * Configuration.CC_ENEMY_SIZES_X[currentSize]);
                    y = r.Next(Utility.ScreenHeight);
                }


                // Build the enemy
                Entity e = new Entity("Minigame/Rollout/LDL");
                e.SetPosition(new Vector2(x, y));
                e.SetScale(Configuration.CC_ENEMY_SIZES_X[currentSize], Configuration.CC_ENEMY_SIZES_Y[currentSize]);

                // Set the flips just in case
                //if (x <= Utility.ScreenWidth / 2) e.sprite.setFlipX(true);
                //else e.sprite.setFlipX(false);

                // Add to the pile
                enemies.Add(e);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            base.Draw(sb);

            foreach (Entity e in enemies)
                e.Draw(sb);

            heart.Draw(sb);
            hunter.Draw(sb);

        }
    }
}