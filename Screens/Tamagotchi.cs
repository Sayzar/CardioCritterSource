using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardioCritters.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using CardioCritters.Code.GUI.Buttons;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;
using CardioCritters.Code.Heart;

namespace CardioCritters.Code.Screens
{
    public class Tamagotchi : Screen
    {
        // Entities specific to this screen
        private Entity background;
        private Pet pet;
        private GestureType[] petActions = {
                                            GestureType.Tap,
                                            GestureType.HorizontalDrag,
                                            GestureType.VerticalDrag,
                                            GestureType.Pinch
                                           };

        // The buttons!
        private NewScreenButton fridge, firstAid, weights, clipboard, closet, door, scale;
        
        // Debug printing
        private String toprint = "TEST";

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

        public Tamagotchi() : base(null)
        {
            // The background is a movable entity
            background = new Entity("Tamagotchi/background");
            background.SetSizeRelativeToScreen(2f, 1f);
            background.SetPosition(0, 0);
            background.renderIndex = 1f;

            // Make all the buttons
            #region Button Initilization

            fridge = new NewScreenButton("Tamagotchi/Furniture/fridge", "Tamagotchi/Furniture/fridge2", "Minigames_Food", false);
            fridge.SetSizeRelativeToScreen(0.25f, 0.5f);
            fridge.SetPosition(2*Utility.ScreenWidth - fridge.dimensions.Width, Utility.ScreenHeight - fridge.dimensions.Height*5/4);

            firstAid = new NewScreenButton("Tamagotchi/Furniture/firstaidkit", "Tamagotchi/Furniture/firstaidkit2", "Minigames_Health", false);
            firstAid.SetSizeRelativeToScreen(0.15f, 0.15f);
            firstAid.SetPosition(2 * Utility.ScreenWidth - fridge.dimensions.Width - firstAid.dimensions.Width, Utility.ScreenHeight - firstAid.dimensions.Height * 2);

            weights = new NewScreenButton("Tamagotchi/Furniture/weights", "Tamagotchi/Furniture/weights2", "Minigames_Workout", false);
            weights.SetSizeRelativeToScreen(0.15f, 0.175f);
            weights.SetPosition(2 * Utility.ScreenWidth - fridge.dimensions.Width - firstAid.dimensions.Width - weights.dimensions.Width, Utility.ScreenHeight - weights.dimensions.Height * 2);

            scale = new NewScreenButton("Tamagotchi/Furniture/scale", "Tamagotchi/Furniture/scale2", "Statistics", false);
            scale.SetSizeRelativeToScreen(0.25f, 0.2f);
            scale.SetPosition(Utility.ScreenWidth/3, Utility.ScreenHeight - scale.dimensions.Height*5/3);

            door = new NewScreenButton("Tamagotchi/Furniture/door2", "Tamagotchi/Furniture/door", "Exit", false);
            door.SetSizeRelativeToScreen(0.15f, 0.4f);
            door.SetPosition(Utility.ScreenWidth/20, Utility.ScreenHeight - door.dimensions.Height*7/5);

            closet = new NewScreenButton("Tamagotchi/Furniture/closet", "Tamagotchi/Furniture/closet2", "Accessories", false);
            closet.SetSizeRelativeToScreen(0.2f, 0.4f);
            closet.SetPosition(Utility.ScreenWidth, Utility.ScreenHeight - closet.dimensions.Width*5/3);

            clipboard = new NewScreenButton("Tamagotchi/Furniture/clipboard", "Tamagotchi/Furniture/clipboard2", "Stress_Journal", false);
            clipboard.SetSizeRelativeToScreen(0.125f, 0.25f);
            clipboard.SetPosition(door.dimensions.X + door.dimensions.Width + Utility.ScreenWidth/100, Utility.ScreenHeight/2 - clipboard.dimensions.Height);

            #endregion

            // The pet!
            pet = new Pet();
            pet.SetSizeRelativeToScreen(0.25f, 0.3f);
            pet.SetPosition(Utility.ScreenWidth / 2, Utility.ScreenHeight - pet.dimensions.Height);

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

            toprint = _cameraPosition.X.ToString();

            if (_cameraPosition.X <= -Utility.ScreenWidth) _cameraPosition.X = -Utility.ScreenWidth;
            if (_cameraPosition.X >= 0) _cameraPosition.X = 0;    
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
#if WINDOWS
            // Some nice little hack to test this screen
            if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Left))
                MoveWorld(-5);
            else if (Utility.inputManager.currentKeyboardState.IsKeyDown(Keys.Right))
                MoveWorld(5);
#endif
            // Update the pet
            pet.Update(gameTime);

            // On any event that occurs while over the pet, do stuff!
            for (int i = 0; i < petActions.Length; i++)
                if (Utility.inputManager.GetGesture(petActions[i])
                    && pet.Intersects(Utility.inputManager.GetTouches()[0].Position - _cameraPosition))
                    pet.HandleInput(petActions[i]);

            // On a drag, we must move the world!
            if (Utility.inputManager.GetGesture(GestureType.HorizontalDrag))
            {
                TouchLocation outTouch;
                Utility.inputManager.GetTouches()[0].TryGetPreviousLocation(out outTouch);
                float deltaX = outTouch.Position.X - Utility.inputManager.GetTouches()[0].Position.X;
                MoveWorld(deltaX);
            }

            // Update the buttons if the pet isnt in the way
            UpdateButtons();

            #region Camera and World Update

            // Update the camera
            _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) *
                    Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            #endregion
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            //Hackssss 
            sb.End();

            // Start the "better" sb
            sb.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, _view);
            
            // Draw the background entity
            background.Draw(sb);

            // Draw the pet
            pet.Draw(sb);

            // Draw all the buttons
            DrawButtons(sb);

            sb.DrawString(Game1.font, toprint, Vector2.Zero, Color.Black);

            // more hacks
            sb.End();
            sb.Begin();
        }

        private void UpdateButtons()
        {
            Vector2 pos = new Vector2(-1,-1);

            foreach (TouchLocation loc in Utility.inputManager.GetTouches())
                if (!pet.Intersects(loc.Position - _cameraPosition))
                    pos = loc.Position - _cameraPosition;       // Subtract to get the world location

            toprint = pos.X + ", " + pos.Y;

            fridge.HandleInput(pos);
            firstAid.HandleInput(pos);
            weights.HandleInput(pos);
            clipboard.HandleInput(pos);
            scale.HandleInput(pos);
            closet.HandleInput(pos);
            door.HandleInput(pos);
        }

        private void DrawButtons(SpriteBatch sb)
        {
            fridge.Draw(sb);
            firstAid.Draw(sb);
            weights.Draw(sb);
            clipboard.Draw(sb);
            scale.Draw(sb);
            closet.Draw(sb);
            door.Draw(sb);
        }
    }
}