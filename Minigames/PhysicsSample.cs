using System;
using System.Collections.Generic;
using CardioCritters.Screens;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CardioCritters.Code.Minigames
{
    public class PhysicsSample : Screen
    {
        private World _world;

        private Body _circleBody;
        private Body _groundBody;

        private Texture2D _circleSprite;
        private Texture2D _groundSprite;

        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;

        // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
        // 1 meters equals 64 pixels here
        // (Objects should be scaled to be between 0.1 and 10 meters in size)
        private const float MeterInPixels = 64f;

        public PhysicsSample()
            : base(null)
        {
            _world = new World(new Vector2(0, 20));
            //Just for now, TODO: thread content load
            LoadContent();
        }

        protected void LoadContent()
        {
            // Initialize camera controls
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;

            _screenCenter = new Vector2(Utility.ScreenWidth / 2f,
                                                Utility.ScreenHeight / 2f);

            // Load sprites
            _circleSprite = Utility.Content.Load<Texture2D>("Heart/Test1"); //  96px x 96px => 1.5m x 1.5m //Rofl test1 as a circle
            _groundSprite = Utility.Content.Load<Texture2D>("Heart/Test2"); // 512px x 64px =>   8m x 1m //Rofl test2 as the ground

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = (_screenCenter / MeterInPixels) + new Vector2(0, -1.5f);

            // Create the circle fixture
            _circleBody = BodyFactory.CreateCircle(_world, 96f / (2f * MeterInPixels), 1f, circlePosition);
            _circleBody.BodyType = BodyType.Dynamic;

            // Give it some bounce and friction
            _circleBody.Restitution = 0.3f;
            _circleBody.Friction = 0.5f;

            /* Ground */
            Vector2 groundPosition = (_screenCenter / MeterInPixels) + new Vector2(0, 1.25f);

            // Create the ground fixture
            _groundBody = BodyFactory.CreateRectangle(_world, 512f / MeterInPixels, 64f / MeterInPixels, 1f, groundPosition);
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;
        }

        public override void Update(GameTime gameTime)
        {
            InputManager input = Utility.inputManager;

            // Switch between circle body and camera control
#if ANDROID
            TouchCollection touches = input.GetTouches();
            if (touches.Count > 1)
            {
#elif WINDOWS
            TouchLocation[] touches = input.GetTouches();
            if (touches.Length > 1)
            {
#endif
                // Move camera
                if (input.GetGesture(GestureType.FreeDrag) || 
                    input.GetGesture(GestureType.HorizontalDrag) ||
                    input.GetGesture(GestureType.VerticalDrag))
                {
                    TouchLocation prevLoc;
                    if (touches[0].TryGetPreviousLocation(out prevLoc))
                    {
                        Vector2 deltaPos = touches[0].Position - prevLoc.Position;
                        _cameraPosition.X += deltaPos.X;
                        _cameraPosition.Y += deltaPos.Y;
                    }
                }
                _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) *
                        Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));
            }
            else
            {
                // We make it possible to rotate the circle body
                if (input.GetGesture(GestureType.Tap))
                {
                    foreach (TouchLocation touch in input.GetTouches())
                    {
                        if (touch.Position.X < _circleBody.Position.X * MeterInPixels)
                        {
                             _circleBody.ApplyTorque(-10);
                            break;
                        }
                        else if (touch.Position.X > _circleBody.Position.X * MeterInPixels)
                        {
                            _circleBody.ApplyTorque(10);
                            break;
                        }
                    }
                }
                //Jump
                if (input.GetGesture(GestureType.DoubleTap))
                    _circleBody.ApplyLinearImpulse(new Vector2(0, -10));
            }

            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }

        public override void Draw(SpriteBatch sb)
        {
            //base.Draw(sb);
            /* Circle position and rotation */
            // Convert physics position (meters) to screen coordinates (pixels)
            Vector2 circlePos = _circleBody.Position * MeterInPixels;
            float circleRotation = _circleBody.Rotation;

            /* Ground position and origin */
            Vector2 groundPos = _groundBody.Position * MeterInPixels;
            Vector2 groundOrigin = new Vector2(512f / 2f, _groundSprite.Height / 2f);

            // Align sprite center to body position
            Vector2 circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);

            //Massive hack
            sb.End();

            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            //Draw circle
            sb.Draw(_circleSprite, circlePos, null, Color.White, circleRotation, circleOrigin, 1f, SpriteEffects.None, 0f);

            //Draw ground
            sb.Draw(_groundSprite, groundPos, new Rectangle(0,0,512,64), Color.White, 0f, groundOrigin, 1f, SpriteEffects.None, 0f);

            sb.End();

            //sb.Begin()
            // sb.Begin();

            // Display instructions
//             sb.DrawString(_font, Text, new Vector2(14f, 14f), Color.Black);
//             sb.DrawString(_font, Text, new Vector2(12f, 12f), Color.White);

           //  sb.End();
        }
    }
}