using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CardioCritters.Screens;
using Microsoft.Xna.Framework.Input.Touch;

using System.Threading;
using XnaUtility;

#if ANDROID
using Android.App;
using System.IO;
using System.Collections.Generic;
#endif

namespace CardioCritters
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static SpriteFont font;

        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content = new ThreadSafeContentManager(Services);
#if WINDOWS
            Content.RootDirectory =  "..\\..\\..\\Content";
            graphics.IsFullScreen = false;
#elif ANDROID
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
#endif
            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;// | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.DoubleTap | GestureType.HorizontalDrag | GestureType.VerticalDrag | GestureType.Hold | GestureType.Pinch;
            Utility.Content = Content;
            Utility.ScreenWidth = GraphicsDevice.Viewport.Width;
            Utility.ScreenHeight = GraphicsDevice.Viewport.Height;
            
            base.Initialize();
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Load thread and play splash screen at the same time!
            //1. Load just enough so splash screen can run
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Utility.AddNewTexture("SplashScreen/splash");
            Utility.AddNewTexture("Misc/boundingBox");

            
            Splash spScreen = new Splash();
            //2. Kick off heavy duty loading thread
            Thread loadContentThread = new Thread(new ParameterizedThreadStart(LoadContentThread));
            loadContentThread.Start(spScreen);
            //3. Main thread continues updating/displaying splash screen until loading is done.
            ScreenStackManager.AddScreen(spScreen);
        }

        private void LoadContentThread(object splashScreen)
        {
#if ANDROID
            font = Content.Load<SpriteFont>("SpriteFont1");
#endif
#if WINDOWS
            font = Content.Load<SpriteFont>("spriteFont1");
#endif
            // Load all the textures at once, and only once (This will eventually become a loading screen if we end up needing it)
            string[] texturesToLoad = {
                                          // Main Menu
#if ANDROID
            "MainMenu/Background",
#endif
#if WINDOWS
            "MainMenu/background",
#endif
                                          
                                          "MainMenu/Buttons/continue", "MainMenu/Buttons/continue2",
                                          "MainMenu/Buttons/credits", "MainMenu/Buttons/credits2", 
                                          "MainMenu/Buttons/newgame","MainMenu/Buttons/newgame2", 
                                          "MainMenu/Buttons/options", "MainMenu/Buttons/options2",
                                          "Tamagotchi/background",
                                          "Tamagotchi/Furniture/fridge", "Tamagotchi/Furniture/fridge2",
                                          "Tamagotchi/Furniture/weights", "Tamagotchi/Furniture/weights2",
                                          "Tamagotchi/Furniture/firstaidkit", "Tamagotchi/Furniture/firstaidkit2",
                                          "Tamagotchi/Furniture/clipboard", "Tamagotchi/Furniture/clipboard2",
                                          "Tamagotchi/Furniture/closet", "Tamagotchi/Furniture/closet2",
                                          "Tamagotchi/Furniture/door", "Tamagotchi/Furniture/door2",
                                          "Tamagotchi/Furniture/scale", "Tamagotchi/Furniture/scale2",
                                          "MinigameSelection/background_excercise", "MinigameSelection/background_hunger",
                                          "MinigameSelection/background_internal", "MinigameSelection/HeftyHeart", "MinigameSelection/HeftyHeart2", 
                                          "MinigameSelection/locked", "MinigameSelection/locked2",
                                          "Minigame/HeftyHeart/background", "Minigame/HeftyHeart/endurance", "Minigame/HeftyHeart/endurance2",
                                          "Minigame/HeftyHeart/strength", "Minigame/HeftyHeart/strength2",
                                          "Minigame/HeftyHeart/instruction", "Minigame/HeftyHeart/instruction2", "Minigame/HeftyHeart/instruction3",
                                          "Minigame/HeftyHeart/gameover_bad", "Minigame/HeftyHeart/gameover_good",
                                          "Minigame/HeftyHeart/good", "Minigame/HeftyHeart/miss", 
                                          "Minigame/HeftyHeart/Lift/1", "Minigame/HeftyHeart/Lift/2", 
                                          "Minigame/HeftyHeart/Lift/3", "Minigame/HeftyHeart/Lift/4",
                                          "Minigame/HeftyHeart/Stretch/idle", "Minigame/HeftyHeart/Stretch/left", "Minigame/HeftyHeart/Stretch/right", "Minigame/HeftyHeart/Stretch/up", "Minigame/HeftyHeart/Stretch/down",
                                          "Minigame/HeftyHeart/Stretch/instruction", "Minigame/HeftyHeart/Lift/instruction",
                                          "Minigame/HeftyHeart/Meter/outerbar", "Minigame/HeftyHeart/Meter/fillbar",
                                          "Minigame/HeftyHeart/Meter/littleheart1", "Minigame/HeftyHeart/Meter/littleheart2", "Minigame/HeftyHeart/Meter/littleheart3"

                                      };

            foreach (string s in texturesToLoad)
                Utility.AddNewTexture(s);

            string[] animationsToLoad = {
                                            "Heart/Idle_Happy/",
                                            "Heart/Idle_Ok/",
                                            "Heart/Idle_Sad/",
                                            "Heart/Tap/",
                                            "Heart/Tickle/"
                                        };

            int[] animationFrames = {
                                        9,
                                        10,
                                        6,
                                        10,
                                        10
                                    };

            for (int i = 0; i < animationsToLoad.Length; i++)
                Utility.LoadAnimation(animationsToLoad[i], animationFrames[i]);
            

            ((Splash)(splashScreen)).IsLoadingThreadDone = true;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }
            // TODO: Add your update logic here
            Utility.inputManager.Update();
            Utility.musicManager.Update(gameTime);

            ScreenStackManager.Update(gameTime);

            //EXAMPLE ON HOW TO USE CROSSPLATFORM GETTOUCHES
            foreach (TouchLocation loc in Utility.inputManager.GetTouches())
            {
                Vector2 pos = loc.Position;
            }
            base.Update(gameTime);
        }

        //Remove me once we no longer need input manager gesture example
        Color clearColor = Color.White;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(clearColor);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            ScreenStackManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
