using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;

#if ANDROID
using Microsoft.Devices.Sensors;
#endif

namespace CardioCritters
{
    public class InputManager
    {
        /// <summary>
        /// Returns true if the gesture specified was detected this frame
        /// </summary>
        /// <param name="gesture">
        /// GestureType.Flick, GestureType.Tap, etc
        /// </param>
        /// <returns></returns>
        public bool GetGesture(GestureType gesture)
        {
            return (currentGestures & gesture) == gesture; //Bitwise AND comparison
        }

#if ANDROID
        Accelerometer accelSensor;
        public Vector3 GetAccelReading()
        {
            return accelSensor.CurrentValue.Acceleration;
        }
        public void StartAccel() { if (accelSensor != null) accelSensor.Start(); }
        public void StopAccel() { if (accelSensor != null) accelSensor.Stop(); }

#elif WINDOWS
        public Vector3 GetAccelReading()
        {
            Vector3 movement = new Vector3();
            int speed = 5;

            // since we're landscape, to make sense we have to do switch X and Y

            if (currentKeyboardState.IsKeyDown(Keys.Up)) movement.X = -speed;
            else if (currentKeyboardState.IsKeyDown(Keys.Down)) movement.X = speed;

            if (currentKeyboardState.IsKeyDown(Keys.Left)) movement.Y = -speed;
            else if (currentKeyboardState.IsKeyDown(Keys.Right)) movement.Y = speed;

            return movement;
        }
#endif

#if ANDROID
        /// <summary>
        /// Returns all touches, contains their Position and state
        /// </summary>
        /// <returns></returns>
        public TouchCollection GetTouches()
        {
            return currentTouchState;
        }
        private TouchCollection currentTouchState;
#elif WINDOWS
        private TouchLocation[] curTouchLocation = new TouchLocation[1];
        /// <summary>
        /// Returns all touches, contains their Position and state
        /// Windows emulation hack
        /// </summary>
        /// <returns></returns>
        public TouchLocation[] GetTouches()
        {
            return curTouchLocation;
        }
#endif

        public KeyboardState currentKeyboardState, previousKeyboardState;
        public GamePadState currentGamePadState;
        
#if WINDOWS
        private MouseState curMouseState;
        private MouseState lastMouseState;
#endif

        GestureType currentGestures;

        public InputManager()
        {
#if ANDROID
            // android stuff initialization
            accelSensor = new Accelerometer();
#elif WINDOWS
            //This crap doesn't work for Windows yet -_-, writing my own touch emulation.
            TouchPanel.EnableMouseGestures = true;
            TouchPanel.EnableMouseTouchPoint = true;
            curTouchLocation[0] = new TouchLocation(5, TouchLocationState.Pressed, new Vector2(0, 0));
#endif
        }

        public void Update()
        {
            currentGestures = 0;

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
#if ANDROID
            currentTouchState = TouchPanel.GetState();
            
#endif
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                currentGestures |= gesture.GestureType; //Bitwise OR
            }
#if WINDOWS //Windows emulation of gestures
            curMouseState = Mouse.GetState();
            if (curMouseState.LeftButton == ButtonState.Pressed)
            {
                if (lastMouseState.LeftButton == ButtonState.Released)
                    currentGestures |= GestureType.Tap;
                else
                {
                    //We're draggin now
                    currentGestures |= GestureType.FreeDrag;
                }
                //TODO: Double click = tap
                //Store left mouse as a touch
                curTouchLocation[0] = new TouchLocation(5, TouchLocationState.Pressed, new Vector2(curMouseState.X, curMouseState.Y));
            }
            if (curMouseState.RightButton == ButtonState.Pressed)
            {
                currentGestures |= GestureType.Pinch;
            }
            lastMouseState = curMouseState;

            
#endif
        }
    }
}