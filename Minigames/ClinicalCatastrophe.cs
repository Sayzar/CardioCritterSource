using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardioCritters.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CardioCritters.Code.Minigames
{
    public class ClinicalCatastrophe : Screen
    {
        private enum FallState
	    {
		    //In the air
		    FALLING,
		    //Bouncing to its final position in the container
		    CONTAINED,
		    //Resting in container
		    RESTING,
		    //Getting flung back out
		    EJECTING,
	    }

        private class FallingObject
	    {
		    private FallState state = FallState.FALLING;

		    public Entity entity;
		
		    public bool isBomb;

            private ClinicalCatastrophe clinicGame;

		    private String bomb = "Minigame/bouncegame/bomb";
		    private String[] heart = new String[]
		                                              {
				    "Minigame/bouncegame/collect1",
				    "Minigame/bouncegame/collect2",
				    "Minigame/bouncegame/collect3",
				    "Minigame/bouncegame/collect4",
				    "Minigame/bouncegame/collect5"
		                                              };

		    public float velX = 0;
		    public float velY = 0;
            private float angVel = 0;

            public Vector2 getCenter() { return entity.GetCenter(); }

            public float posX;
            public float posY;
		    public Vector2 getPosition() { return new Vector2(posX, posY); }

		    private float angle = 0;

		    public bool Kill()
		    {
			    return posY + getRadius() < 0 || state == FallState.RESTING;
		    }

		    public float getRadius()
		    {
			    if (isBomb) return 60;
			    else 	    return 60;
		    }

		    public FallingObject(ClinicalCatastrophe clinic)
		    {
                clinicGame = clinic;
			    CreateObject(generator.NextDouble() <= BOMBCHANCE);
		    }

            public FallingObject(ClinicalCatastrophe clinic, bool isBomb)
		    {
                clinicGame = clinic;
			    CreateObject(isBomb);
		    }

            private void CreateObject(bool isBomb) 
            {
                posX = clinicGame.spawnPoint.X;
                posY = clinicGame.spawnPoint.Y;

                this.isBomb = isBomb;
			    if (isBomb)
				    entity = new Entity(bomb);
			    else
				    entity = new Entity(heart[generator.Next(heart.Length)]);

                entity.SetPosition(clinicGame.spawnPoint);

			    //entity.sprite.setAnchorPoint(0.5f, 0.5f);
			
			    posX = (float) generator.NextDouble() * Utility.ScreenWidth;

			    angVel = (float) generator.NextDouble();
            }

		    public void CleanUp()
		    {
			    //entity.sprite.removeFromParentAndCleanup(true);
		    }

		    public void splode()
		    {
			    velY = ejectVelocity;
			    state = FallState.EJECTING;
		    }

		    public void update(float elapsed)
		    {
			    //Accelerate
			    velY += gravity * elapsed;

			    Vector2 oldpos = getPosition();

			    //Move
			    posX += velX * elapsed;
			    posY += velY * elapsed;
			    entity.SetPosition(new Vector2(posX, posY));

                Vector2 newpos = getPosition();

			    //Rotate
			    angle += angVel;
			    //entity.sprite.setRotation(angle);

			    //States
			    if (state == FallState.FALLING)
			    {
				    if (newpos.Y > clinicGame.jarTop())
				    {
                        if (newpos.X < clinicGame.jarLeft() + getRadius()) velX = -Math.Abs(velX);
                        if (newpos.X > clinicGame.jarRight() - getRadius()) velX = Math.Abs(velX);
				    }
				
			
				
				    //Check if it passed through the top of the jar
                    if (oldpos.Y < clinicGame.jarTop() && newpos.Y > clinicGame.jarTop()
                        && newpos.X >= clinicGame.jarLeft() && newpos.X <= clinicGame.jarRight())
				    {
					    //Log.i("Tag", "Enter jar");
					    state = FallState.CONTAINED;
				    }
			    }
			    else if (state == FallState.CONTAINED)
			    {
				    //Bounce around
                    if (newpos.X < clinicGame.jarLeft() + getRadius()) velX = Math.Abs(velX);
                    if (newpos.X > clinicGame.jarRight() - getRadius()) velX = -Math.Abs(velX);

				    //checks if entity is tapped out container
                    if (newpos.Y < clinicGame.jarTop()) state = FallState.FALLING;
				
				    //Check if it hit the bottom
                    if (newpos.Y > clinicGame.jarBottom())
				    {
					    if (isBomb)
					    {
						    //Splode

						    clinicGame.removeScore();
					    }
					    else
					    {
						    clinicGame.addScore();
					    }
					    state = FallState.RESTING;
				    }
			    }
		    }
	    }
        
	    private void addScore()
	    {
		    currentScore++;
		    if (currentScore > scoreToWin)
		    {
			    //CCDirector.sharedDirector().replaceScene(
				//	    GameOver.scene(new Entity("Minigame/bouncegame/gameover"),
				//			    new Vector2(2*Utility.ScreenWidth/3, Utility.ScreenHeight/6),
				//			    currentScore));
			    //SoundEngine.sharedEngine().stopSound();
			    //SoundEngine.sharedEngine().realesAllSounds();
			    currentScore = scoreToWin;
		    }
		    updateFill();
	    }

        private void removeScore()
	    {
		    currentScore -= BOMBSKILL;
		    if (currentScore < 0) currentScore = 0;
		    updateFill();
	    }
	
	    private void updateFill()
	    {
		    fill.SetPosition(new Vector2(container.dimensions.X, 
                container.dimensions.Bottom -fill.dimensions.Height*(currentScore/scoreToWin)));
	    }

	    private int currentScore = 0;
	    private const int scoreToWin = 20;

	    private const float restBounceMax = 50;
	    private const float ejectVelocity = 400;

	    private const int BOMBSKILL = 3;

        // bomb collision fling velocity
        private const float flingVel = 200;

	    private Vector2 spawnPoint = new Vector2(Utility.ScreenWidth * 0.75f, 0); //HACK
	    private const float gravity = 50;

	    private Vector2 touchStartPoint = new Vector2(0, 0);
	    private Vector2 touchEndPoint;
	    private float touchLeft() { return Math.Min(touchStartPoint.X, touchEndPoint.X); }
	    private float touchRight() { return Math.Max(touchStartPoint.X, touchEndPoint.X); }
	    private TimeSpan lineTimeToDie;
	    private const int lineDuration = 1000;

	    private const float BOMBCHANCE = 0.4f;

	    private const int spawnIntervalMin = 800;
	    private const int spawnIntervalMax = 1700;
	    private TimeSpan timeToNextSpawn;

	    //private bool lineActive() { return lineTimeToDie > 0; }
	
	    private List<FallingObject> objects;
	    private Entity container;
	    //private Entity background;
	    private Entity fill;
	    private Entity field;
	    private TimeSpan timer = new TimeSpan();
	    private static Random generator = new Random();

        private float jarTop() { return container.dimensions.Top; }
        private float jarLeft() { return container.dimensions.Left; }
        private float jarRight() { return container.dimensions.Right; }
        private float jarBottom() { return container.dimensions.Bottom; }

	    private Entity line;
	    private float lineAngle;
	    private float lineOffX() { return touchStartPoint.X - touchEndPoint.X; }
	    private float lineOffY() { return touchStartPoint.Y - touchEndPoint.Y; }
	    private float lineNormX() { return (float)Math.Cos(lineAngle); }
	    private float lineNormY() { return (float)Math.Sin(lineAngle); }



        public ClinicalCatastrophe()
            : base("Minigame/bouncegame/bg")
        {
            //this.setIsTouchEnabled(true);
		    //this.setIsAccelerometerEnabled(true);
		
		    //SoundEngine.sharedEngine().playSound(CCDirector.sharedDirector().getActivity(), R.raw.happyflowers, true);

		    timeToNextSpawn = new TimeSpan(0,0,1);

		    objects = new List<ClinicalCatastrophe.FallingObject>();

            /*
		    background = new Entity("Minigame/bouncegame/bg");
		    background.SetScale(
				    Utility.ScreenWidth / background.sprite.Width,
				    Utility.ScreenHeight / background.sprite.Height);
            */

		    container = new Entity("Minigame/bouncegame/bin");
            container.SetSizeRelativeToScreen(0.4f, 0.3f);
            container.SetPosition(50, Utility.ScreenHeight - container.dimensions.Height);
            //container.sprite.setVertexZ(-10);

            fill = new Entity("Minigame/bouncegame/fill");
            container.SetSizeRelativeToScreen(0.4f, 0.3f);
            fill.SetPosition(container.dimensions.X, container.dimensions.Bottom);

		    field = new Entity( "Minigame/bouncegame/field");
		    //field.sprite.setAnchorPoint(0.5f, 0.5f);

		    line = new Entity("Minigame/bouncegame/line");
		    //line.sprite.setVisible(false);
		    //line.sprite.setAnchorPoint(0.5f, 0);

		    //this.schedule("update");
        }

        public override void Update(GameTime gameTime)
        {

            timer -= gameTime.ElapsedGameTime;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            DetectCollision();

            timeToNextSpawn -= gameTime.ElapsedGameTime;
			if (timeToNextSpawn <= TimeSpan.Zero)
			{
				//Spawn a new thingummy
				timeToNextSpawn = new TimeSpan(0,0,0,0, (int) generator.NextDouble() * (spawnIntervalMax - spawnIntervalMin) + spawnIntervalMin);
				objects.Add(new FallingObject(this));
			}


            lineTimeToDie -= gameTime.ElapsedGameTime;
			if (lineTimeToDie <= TimeSpan.Zero)
			{
				//Hide line
				field.opacity = 0f;
			}
			else
				field.opacity = (((float) lineTimeToDie.TotalMilliseconds) / lineDuration);
		    

		    //Update objects
		    foreach (FallingObject f in objects) f.update(dt);

		    //Kill objects
		    for (int c = objects.Count -1; c >= 0; c--)
		    {
			    if (objects[c].Kill())
			    {
				    objects[c].CleanUp();
				    objects.RemoveAt(c);
			    }
		    }
        }
        
        public void DetectCollision()
	    {
            Vector2 point = Vector2.Zero;
            foreach (TouchLocation loc in Utility.inputManager.GetTouches())
            {
                point = loc.Position;
            }

		    touchStartPoint = point;

            if (point != Vector2.Zero)
            {
                // the red glow thingy for pressing
                lineTimeToDie = new TimeSpan(0, 0, 0, 0, lineDuration);
                field.SetPosition(point - new Vector2(field.dimensions.Width / 2, field.dimensions.Height / 2));
                field.opacity = 1f;

                foreach (FallingObject o in objects)
                {
                    if (Math.Sqrt(Math.Pow(o.getCenter().X - point.X, 2) + Math.Pow(o.getCenter().Y - point.Y, 2)) < 120)
                    {
                        float vx = o.posX - point.X;
                        float vy = o.posY - point.Y;
                        vx /= Math.Abs(vx) + Math.Abs(vy);
                        vy /= Math.Abs(vx) + Math.Abs(vy);
                        vx *= flingVel;
                        vy *= flingVel;
                        o.velX = vx;
                        o.velY = vy;
                    }
                }
            }
	    }
         

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            base.Draw(sb);

            container.Draw(sb);
            fill.Draw(sb);
            field.Draw(sb);

            foreach (FallingObject o in objects)
                o.entity.Draw(sb);
        }
    }
}