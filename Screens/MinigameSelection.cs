using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardioCritters.Screens;
using CardioCritters.Code.GUI.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;

namespace CardioCritters.Code.Screens
{
    public class MinigameSelection : Screen
    {
        public enum MinigameType
        {
            ENERGY,
            FULLNESS,
            INTERNAL
        };

        // Basic stuff needed for drawing
        private MinigameType type;
        private Entity background, coin;
        private NewScreenButton[] buttons;
        private bool[] locked;

        // Configuration stuff
        private int ROWS = 2;
        private float MINIGAME_WIDTH = Utility.ScreenWidth / 4;
        private float MINIGAME_GAP = Utility.ScreenWidth / 16;
        private float MINIGAME_HEIGHT;

        // The world we are in now
        private World _world;

        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;

        // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
        // 1 meters equals 64 pixels here
        // (Objects should be scaled to be between 0.1 and 10 meters in size)
        private const float MeterInPixels = 64f;

        public MinigameSelection(MinigameType type, String[] minigameNames, bool[] locked)
            : base(null)
        {
            // Needed for certain differences if that's what we're doing
            this.type = type;
            this.locked = locked;
            this.MINIGAME_HEIGHT = (Utility.ScreenHeight - MINIGAME_GAP * (ROWS + 1)) / ROWS;
            
            // Make the background as big as needed
            switch (type)
            {
                case MinigameType.ENERGY:
                    background = new Entity("MinigameSelection/background_excercise");
                    break;
                case MinigameType.FULLNESS: 
                    background = new Entity("MinigameSelection/background_hunger");
                    break;
                case MinigameType.INTERNAL:
                    background = new Entity("MinigameSelection/background_internal");
                    break;
            }
            
            float sizeX = MINIGAME_GAP * (minigameNames.Length/ROWS + 1) + MINIGAME_WIDTH * minigameNames.Length/ROWS;
            background.SetSizeRelativeToScreen(sizeX / Utility.ScreenWidth, Utility.ScreenHeight);
            background.renderIndex = 1f;

            //coin = new Entity("MinigameSelection/coin");
            //coin.SetSizeRelativeToScreen(0.05f, 0.075f);
            //coin.SetPosition(Vector2.Zero);

            // Now load enough of the assets to make the buttons
            buttons = new NewScreenButton[minigameNames.Length];
            String prefix = "MinigameSelection/";
            for (int i = 0; i < minigameNames.Length; i++)
            {
                buttons[i] = locked[i] ? 
                    new NewScreenButton(prefix + "locked", prefix + "locked2", "", false):
                    new NewScreenButton(prefix + minigameNames[i], prefix + minigameNames[i] + "2", minigameNames[i], true);
                
                Vector2 pos = new Vector2(MINIGAME_GAP * (i/ROWS + 1) + MINIGAME_WIDTH * (i/ROWS),
                                        ((i % ROWS) + 1) * MINIGAME_GAP + (i % ROWS) * MINIGAME_HEIGHT);
                
                buttons[i].renderIndex = 0.9f;
                buttons[i].SetSizeRelativeToScreen(MINIGAME_WIDTH / Utility.ScreenWidth, MINIGAME_HEIGHT / Utility.ScreenHeight);
                buttons[i].SetPosition(pos);
            }

            // Initialize the world
            _world = new World(new Vector2(0, 20));
            LoadContent();
        }

        protected void LoadContent()
        {
            // Initialize camera controls
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;

            _screenCenter = new Vector2(Utility.ScreenWidth / 2f,
                                                Utility.ScreenHeight / 2f);
        }

        private void MoveWorld(float deltaX)
        {
            // Move the background
            _cameraPosition += new Vector2(-1 * deltaX, 0);

            if (_cameraPosition.X <= - (background.dimensions.Width - Utility.ScreenWidth) ) _cameraPosition.X = - (background.dimensions.Width - Utility.ScreenWidth);
            if (_cameraPosition.X >= 0) _cameraPosition.X = 0;
        }

        public override void Update(GameTime gameTime)
        {
// Move the world block
#if WINDOWS
         // Some nice little hack to test this screen
            if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Left))
                MoveWorld(-5);
            else if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Right))
                MoveWorld(5);   

            if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Back))
                ScreenStackManager.PopScene();
#endif
            Vector2 pos = new Vector2(-1, -1);

            foreach (TouchLocation loc in Utility.inputManager.GetTouches())
                pos = loc.Position - _cameraPosition;

            for (int i = 0; i < buttons.Length; i++)
                buttons[i].HandleInput(pos);

            // On a drag, we must move the world!
            if (Utility.inputManager.GetGesture(GestureType.HorizontalDrag))
            {
                TouchLocation outTouch;
                Utility.inputManager.GetTouches()[0].TryGetPreviousLocation(out outTouch);
                float deltaX = outTouch.Position.X - Utility.inputManager.GetTouches()[0].Position.X;
                MoveWorld(deltaX);
            }

            #region Camera and World Update

            // Update the camera
            _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) *
                    Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            #endregion
        }

        public override void Draw(SpriteBatch sb)
        {
            //Hackssss 
            sb.End();

            // Start the "better" sb
            sb.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, _view);

            background.Draw(sb);
            
            //coin.Draw(sb);

            for (int i = 0; i < buttons.Length; i++)
                buttons[i].Draw(sb);

            sb.End();
            sb.Begin();
        }
    }
}